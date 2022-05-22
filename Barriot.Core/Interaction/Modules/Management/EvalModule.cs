using Barriot.Extensions;
using Barriot.Interaction.Attributes;
using Barriot.Interaction.Modals;
using Barriot.Models;
using MailKit.Net.Smtp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MimeKit;
using System.Diagnostics;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    public class EvalModule : BarriotModuleBase
    {
        private readonly IConfiguration _configuration;

        public EvalModule(IConfiguration config)
            => _configuration = config;

        [SlashCommand("evaluate", "Evaluates/Runs a C# script.")]
        public async Task AuthorizeAsync()
        {
            var data = _configuration.GetSection("MailAuthentication");

            if (Context.User.Id == data.GetValue<ulong>("Id"))
            {
                if (!SecurityExtensions.InSudo)
                {
                    var key = SecurityExtensions.GenerateValidationKey();

                    var salt = SecurityExtensions.GenerateSalt();

                    await RespondWithModalAsync<EvalModal>($"eval-run:{SecurityExtensions.ComputeHash(key, salt)},{salt}");

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
                    await RespondWithModalAsync<SudoEvalModal>("eval-sudo");
            }
            else
                await RespondAsync(
                    text: ":x: **You are not allowed to run this command!**",
                    ephemeral: true);
        }

        [ModalInteraction("eval-sudo")]
        public async Task EvaluateInSudoAsync(SudoEvalModal modal)
        {
            await DeferAsync(true);
            await EvaluateAsync(modal.Script, modal.References, modal.Usings);
        }

        [ModalInteraction("eval-run:*,*")]
        public async Task EvaluateAsync(string hash, string salt, EvalModal modal)
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
                await EvaluateAsync(modal.Script, modal.References, modal.Usings);
            }
        }

        public async Task EvaluateAsync(string script, string references, string usings)
        {
            var options = ScriptOptions.Default
                .WithImports("System", "System.IO", "System.Math", "System.Threading.Tasks", "System.Threading");

            if (references != string.Empty)
            {
                var range = references.Split(',').Select(x => x.Trim());

                if (range.Any(x => x.Equals("Framework.dll")))
                {
                    await FollowupAsync(
                        text: ":x: **You are not allowed to refer to the native codebase in evaluation!**",
                        ephemeral: true);
                    return;
                }

                options.WithReferences(range);
            }

            if (usings != string.Empty)
            {
                var range = usings.Split(',').Select(x => x.Trim());

                if (range.Any(x => x.Contains("Barriot")))
                {
                    await FollowupAsync(
                        text: ":x: **You are not allowed to refer to the native codebase in evaluation!**",
                        ephemeral: true);
                    return;
                }

                options.WithImports(usings);
            }

            if (script == string.Empty)
                await FollowupAsync(
                    text: ":x: **An empty script cannot be evaluated!**",
                    ephemeral: true);

            else
            {
                var context = new EvalContext(Context);
                var stopwatch = new Stopwatch();

                TimeSpan compileTime;
                IEnumerable<Diagnostic> diagnostics;
                var code = CSharpScript.Create(script, options, typeof(EvalContext));

                try
                {
                    stopwatch.Start();
                    diagnostics = code.Compile();
                    stopwatch.Stop();

                    compileTime = stopwatch.Elapsed;
                }
                catch (CompilationErrorException ex)
                {
                    stopwatch.Stop();
                    await FollowupAsync(
                        text: $":x: **Failed to compile your script:**\n\n> {ex.Message}",
                        ephemeral: true);
                    return;
                }

                try
                {
                    stopwatch.Restart();
                    await code.RunAsync(context);
                    stopwatch.Stop();
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    await FollowupAsync(
                        text: $":x: **Failed to run your script, a runtime error has occured:**\n\n {ex.Message}",
                        ephemeral: true);
                    return;
                }

                var eb = new EmbedBuilder()
                    .WithColor(Context.Member.Color)
                    .WithDescription($"```cs\n{script}\n```")
                    .AddField("Result:", context.Result ?? "None.")
                    .AddField("Compile time:", $"{compileTime.Milliseconds} ms ({compileTime.Ticks} ms/t)")
                    .AddField("Execution time:", $"{stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks} ms/t)");

                if (diagnostics.Any())
                    eb.AddField("Diagnostics:", string.Join("\n", diagnostics.Select(x => $"{x.Id}: {x.DefaultSeverity}")));

                await FollowupAsync(
                    text: ":white_check_mark: **Script ran successfully!** *Displayed below is the full output of the evaluation.*",
                    embed: eb.Build(),
                    ephemeral: true);
            }
        }
    }
}
