using Barriot.Application.Interactions.Attributes;

namespace Barriot.Application.Interactions
{
    public class InteractionApiManager
    {
        private readonly Func<ICommandInfo, bool> _fetchAttributeFunc = x => x.Attributes.Any(x => x is AllowAPIAttribute attr && attr.AllowAPI is true);

        private readonly List<string> _commandMap = new();

        private readonly InteractionService _service;

        private const char _filter = ':';

        private const char _wildCardSeperator = '*';

        /// <summary>
        ///     Resembles the predicate check that controls API-logged command value population.
        /// </summary>
        public Func<InteractionProperties, bool> Predicate { get; }

        public InteractionApiManager(InteractionService service)
        {
            _service = service;
            Populate();

            Predicate = x =>
            {
                var name = string.IsNullOrEmpty(x.Name)
                    ? x.CustomId.Split(_filter)[0]
                    : x.Name;

                if (_commandMap.Any(c => c == name))
                    return true;

                return false;
            };
        }

        private void Populate()
        {
            foreach (var command in _service.SlashCommands)
                if (_fetchAttributeFunc(command))
                    _commandMap.Add(command.Name);

            foreach (var command in _service.ContextCommands)
                if (_fetchAttributeFunc(command))
                    _commandMap.Add(command.Name);

            foreach (var component in _service.ComponentCommands)
                if (_fetchAttributeFunc(component))
                {
                    var cid = component.Name;
                    if (cid.Contains(_wildCardSeperator))
                        _commandMap.Add(cid.Split(_filter)[0]);
                    else
                        _commandMap.Add(cid);
                }

            foreach (var modal in _service.ModalCommands)
                if (_fetchAttributeFunc(modal))
                {
                    var cid = modal.Name;
                    if (cid.Contains(_wildCardSeperator))
                        _commandMap.Add(cid.Split(_filter)[0]);
                    else
                        _commandMap.Add(cid);
                }
        }
    }
}
