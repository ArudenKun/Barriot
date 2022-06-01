using Barriot.Extensions;
using Barriot.Interaction.Attributes;
using Barriot.Interaction.Modals;
using Barriot.Models.Services;
using MailKit.Net.Smtp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MimeKit;
using System.Diagnostics;
using System.Text;

namespace Barriot.Interaction.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    public class EvaluationModule : BarriotModuleBase
    {
        private readonly IConfiguration _configuration;

        public EvaluationModule(IConfiguration config)
            => _configuration = config;

        [SlashCommand("evaluate", "Evaluates/Runs a C# script.")]
        public async Task AuthorizeAsync()
        {
            var data = _configuration.GetSection("MailAuthentication");

            if (Context.User.Id == data.GetValue<ulong>("Id"))
            {
                var mb = new ModalBuilder()
                    .WithTitle("Run an evaluation script:")
                    .AddTextInput("Code to evaluate", "code", TextInputStyle.Paragraph, "Print(1);", 1, 4000, true)
                    .AddTextInput("Imports", "imports", TextInputStyle.Short, "Discord, Discord.Rest", 0, 4000, false)
                    .AddTextInput("References", "references", TextInputStyle.Short, "Discord.Net.Core.dll, Discord.Net.Rest.dll", 0, 4000, false)
                    .WithCustomId("eval-sudo");

                if (!SecurityExtensions.InSudo)
                {
                    var key = SecurityExtensions.GenerateValidationKey();

                    var salt = SecurityExtensions.GenerateSalt();

                    mb.AddTextInput("Verification code", "verification", TextInputStyle.Short, "", 1, 6, true);
                    mb.WithCustomId($"eval-run:{SecurityExtensions.ComputeHash(key, salt)},{salt}");

                    await RespondWithModalAsync(mb.Build());

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Rozen", data["Source"]));
                    message.To.Add(new MailboxAddress("Armano den Boef", data["Serving"]));
                    message.Subject = "Evaluate script verification";
                    message.Body = new TextPart("plain")
                    {
                        Text = $"Evaluation verification code: {key}"
                    };

                    using var client = new SmtpClient();

                    client.Connect(data["Server"], 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(data["Username"], data["Password"]);
                    client.Send(message);
                    client.Disconnect(true);
                }
                else
                    await RespondWithModalAsync(mb.Build());
            }
            else
                await RespondAsync(
                    text: ":x: **You are not allowed to run this command!**",
                    ephemeral: true);
        }

        [ModalInteraction("eval-sudo")]
        public async Task EvaluateInSudoAsync(EvaluationModal modal)
        {
            await DeferAsync(true);
            await EvaluateAsync(modal.Script, modal.References, modal.Imports);
        }

        [ModalInteraction("eval-run:*,*")]
        public async Task EvaluateAsync(string hash, string salt, EvaluationModal modal)
        {
            await DeferAsync(true);

            if (hash != SecurityExtensions.ComputeHash(modal.Verification, salt))
            {
                await FollowupAsync(
                    text: ":x: **Invalid authorization code; Evaluation abandoned.**",
                    ephemeral: true);
                return;
            }
            else
            {
                SecurityExtensions.EnableSudoMode();
                await EvaluateAsync(modal.Script, modal.References, modal.Imports);
            }
        }

        public async Task EvaluateAsync(string script, string references, string imports)
        {
            var defaultImports = new string[6]
            {
                "System",
                "System.IO",
                "System.Math",
                "System.Threading.Tasks",
                "System.Threading",
                "System.Linq"
            };

            var options = ScriptOptions.Default
                .WithImports(defaultImports);

            var eb = new EmbedBuilder()
                .WithDescription($"```cs\n{script}\n```");

            if (references != string.Empty)
            {
                var range = references.Split(',').Select(x => x.Trim());

                if (range.Any(x => x.Equals("Framework.dll")))
                {
                    await FollowupAsync(
                        format: MessageFormat.Failure,
                        header: "You are not allowed to refer to the native codebase in evaluation!",
                        embed: eb);
                    return;
                }

                options = options.AddReferences(range);

                eb.AddField("References", references);
            }

            if (imports != string.Empty)
            {
                var range = imports.Split(',').Select(x => x.Trim());

                if (range.Any(x => x.Contains("Barriot")))
                {
                    await FollowupAsync(
                        format: MessageFormat.Failure,
                        header: "You are not allowed to refer to the native codebase in evaluation!",
                        embed: eb);
                    return;
                }

                options = options.AddImports(imports);

                eb.AddField("Imports", string.Join(", ", range.Concat(defaultImports)));
            }

            if (script == string.Empty)
                await FollowupAsync(
                    format: MessageFormat.Failure,
                    header: "An empty script cannot be evaluated!",
                    embed: eb);

            else
            {
                var context = new EvaluationArgs(Context);
                var stopwatch = new Stopwatch();

                IEnumerable<Diagnostic> diagnostics;
                var code = CSharpScript.Create(script, options, typeof(EvaluationArgs));

                try
                {
                    stopwatch.Start();
                    diagnostics = code.Compile();
                    stopwatch.Stop();

                    eb.AddField("Compile time:", $"{stopwatch.Elapsed.Milliseconds} ms ({stopwatch.Elapsed.Ticks} ms/t)");

                    if (diagnostics.Any())
                        eb.AddField("Diagnostics:", string.Join("\n", diagnostics.Select(x => $"{x.Id}: {x.DefaultSeverity}")));
                }
                catch (CompilationErrorException ex)
                {
                    stopwatch.Stop();

                    var exception = ex.ToString();

                    if (exception.Length > 1020)
                    {
                        using var memStream = new MemoryStream(Encoding.ASCII.GetBytes(exception));

                        await FollowupWithFileAsync(
                            fileStream: memStream,
                            fileName: "exception.txt",
                            text: $":x: **Failed to compile your script:**",
                            embed: eb.Build(),
                            ephemeral: true);
                    }
                    await FollowupAsync(
                        format: MessageFormat.Failure,
                        header: "Failed to compile your script!",
                        context: ex.Message,
                        embed: eb);
                    return;
                }

                try
                {
                    stopwatch.Restart();
                    await code.RunAsync(context);
                    stopwatch.Stop();

                    eb.AddField("Execution time:", $"{stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks} ms/t)");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    var exception = ex.ToString();

                    if (exception.Length > 1020)
                    {
                        eb.WithColor(Context.Member.Color);
                        eb.WithFooter("Brought to you by Rozen");

                        using var memStream = new MemoryStream(Encoding.ASCII.GetBytes(exception));

                        await FollowupWithFileAsync(
                            fileStream: memStream,
                            fileName: "exception.txt",
                            text: $":x: **Failed to run your script, a runtime error has occured.**",
                            embed: eb.Build(),
                            ephemeral: true);
                    }
                    else
                    {
                        eb.AddField("Exception:", exception);

                        await FollowupAsync(
                            format: MessageFormat.Failure,
                            header: "Failed to run your script, a runtime error has occured.",
                            context: ex.Message,
                            embed: eb);
                    }
                    return;
                }

                eb.AddField("Result:", context.Result ?? "None.");

                await FollowupAsync(
                    format: MessageFormat.Success,
                    header: "Script ran successfully!",
                    context: "Displayed below is the full output of the evaluation.",
                    embed: eb);
            }
        }
    }
}
