using Barriot.API.Translation;
using Barriot.Caching;
using Barriot.Interaction.Attributes;
using Barriot.Interaction.Services;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class TranslateModule : BarriotModuleBase
    {
        private readonly TranslateService _service;

        public TranslateModule(TranslateService service)
        {
            _service = service;
        }

        [MessageCommand("Translate")]
        public async Task TranslateAsync(IMessage message)
        {
            await DeferAsync(Context.Member.DoEphemeral);

            if (Context.Member.PreferredLang is null)
                Context.Member.PreferredLang = $"en|English";
            string[] args = Context.Member.PreferredLang.Split('|');
            var cb = new ComponentBuilder()
                .WithButton("Change preferred language", $"language-changing:{Context.User.Id},{args[0]}");

            await FollowupAsync(
                text: $":loudspeaker: **Translated text to {args[1]}:** \n\n> {_service.TranslateAsync(args[0], message.CleanContent)}",
                components: cb.Build());
        }

        [DoUserCheck]
        [ComponentInteraction("language-changing:*")]
        public async Task ChangingLanguageAsync(string _)
        {
            var options = await _service.GetSupportedLanguagesAsync();

            var cb = new ComponentBuilder();
            for (int i = 0; i < options.Count; i++)
            {
                var languages = options[i];
                cb.WithButton(
                    label: $"{languages.First().Name[0]} - {languages.Last().Name[0]}",
                    customId: $"language-process:{Context.User.Id},{i}",
                    style: ButtonStyle.Secondary);
            }

            await RespondAsync(
                text: $":speech_balloon: **What do you want your default translation language to be?** *Click your current language ({Context.Member.PreferredLang.Split('|')[1]}) to ignore.*",
                components: cb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("language-process:*,*")]
        public async Task ProcessLanguageAsync(string _, int index)
        {
            var options = (await _service.GetSupportedLanguagesAsync())[index];

            var cb = new ComponentBuilder();
            var sb = new SelectMenuBuilder()
                .WithCustomId($"language-changed:{Context.User.Id}")
                .WithPlaceholder("Click to select a language")
                .WithMinValues(1)
                .WithMaxValues(1);
            foreach (var opt in options)
                sb.AddOption(opt.Name, opt.Code);
            cb.WithSelectMenu(sb);

            await RespondAsync(
                text: $":speech_balloon: **What do you want your default translation language to be?** *Click your current language ({Context.Member.PreferredLang.Split('|')[1]}) to ignore.*",
                components: cb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("language-changed:*")]
        public async Task ChangedLanguageAsync(string _, string[] selectedLang)
        {
            var @new = (await _service.GetSupportedLanguagesAsync()).SelectMany(x => x)
                .First(x => x.Code == selectedLang[0]);

            if (@new.Code == Context.Member.PreferredLang.Split('|')[0])
                await RespondAsync(
                    text: ":x: **Canceled selection!** Your preferred language remains the same.",
                    ephemeral: Context.Member.DoEphemeral);

            else
            {
                await RespondAsync(
                    text: $":white_check_mark: **Succesfully changed target language!** The translate command will now respond in {@new.Name}.",
                    ephemeral: Context.Member.DoEphemeral);

                Context.Member.PreferredLang = $"{@new.Code}|{@new.Name}";
            }
        }
    }
}
