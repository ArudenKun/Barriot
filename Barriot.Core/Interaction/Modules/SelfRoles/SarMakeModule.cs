namespace Barriot.Interaction.Modules.SelfRoles
{
    public class SarMakeModule : BarriotModuleBase
    {
        [ComponentInteraction("sar-new")]
        public async Task MakeAsync()
        {

        }

        [ComponentInteraction("sar-from-message")]
        public async Task MakeFromMessageAsync()
        {

        }

        [ComponentInteraction("sar-from-message-selected")]
        public async Task MessageSelectedAsync(ulong[] selectedValues)
        {

        }

        [ComponentInteraction("sar-from-message-confirm")]
        public async Task ConfirmFromMessageAsync()
        {

        }

        [ComponentInteraction("sar-new-message")]
        public async Task MakeNewMessageAsync()
        {

        }

        [ComponentInteraction("sar-new-message-confirm")]
        public async Task ConfirmNewMessageAsync()
        {

        }
    }
}
