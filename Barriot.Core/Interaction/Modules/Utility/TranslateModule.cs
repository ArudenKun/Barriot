﻿using Barriot.API.Translation;
using Barriot.Caching;
using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class TranslateModule : BarriotModuleBase
    {
        private readonly ITranslateClient _translator;

        private readonly TranslationCache _cache;

        public TranslateModule(ITranslateClient translator, TranslationCache cache)
        {
            _translator = translator;
            _cache = cache;
        }

        [MessageCommand("Translate")]
        public async Task TranslateAsync(IMessage message)
        {
            await DeferAsync(Context.UserData.DoEphemeral);

            if (Context.UserData.PreferredLang is null)
                Context.UserData.PreferredLang = $"en|English";
            string[] args = Context.UserData.PreferredLang.Split('|');
            var cb = new ComponentBuilder()
                .WithButton("Change preferred language", $"language-changing:{Context.User.Id},{args[0]}");
            
            var result = await _translator.TranslateAsync(x =>
            {
                x.ApiKey = "";
                x.Source = "auto";
                x.Target = args[0];
                x.Text = message.CleanContent;
            });

            await FollowupAsync(
                text: $":loudspeaker: **Translated text to {args[1]}:** \n\n> {result}",
                components: cb.Build());
        }

        [DoUserCheck]
        [ComponentInteraction("language-changing:*")]
        public async Task ChangingLanguageAsync(string _)
        {
            var options = await _cache.GetAllLanguagesAsync();

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
                text: $":speech_balloon: **What do you want your default translation language to be?** *Click your current language ({Context.UserData.PreferredLang.Split('|')[1]}) to ignore.*",
                components: cb.Build(),
                ephemeral: Context.UserData.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("language-process:*,*")]
        public async Task ProcessLanguageAsync(string _, int index)
        {
            var options = (await _cache.GetAllLanguagesAsync())[index];

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
                text: $":speech_balloon: **What do you want your default translation language to be?** *Click your current language ({Context.UserData.PreferredLang.Split('|')[1]}) to ignore.*",
                components: cb.Build(),
                ephemeral: Context.UserData.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("language-changed:*")]
        public async Task ChangedLanguageAsync(string _, string[] selectedLang)
        {
            var @new = (await _translator.GetSupportedLanguagesAsync())
                .First(x => x.Code == selectedLang[0]);

            if (@new.Code == Context.UserData.PreferredLang.Split('|')[0])
                await RespondAsync(
                    text: ":x: **Canceled selection!** Your preferred language remains the same.",
                    ephemeral: Context.UserData.DoEphemeral);

            else
            {
                await RespondAsync(
                    text: $":white_check_mark: **Succesfully changed target language!** The translate command will now respond in {@new.Name}.",
                    ephemeral: Context.UserData.DoEphemeral);

                Context.UserData.PreferredLang = $"{@new.Code}|{@new.Name}";
            }
        }
    }
}