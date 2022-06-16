using Barriot.Interaction.Attributes;
using System.Linq.Expressions;

namespace Barriot.Interaction
{
    public sealed class ApiController
    {
        private readonly Expression<Func<ICommandInfo, bool>> _fetchAttributeFunc = x => x.Attributes.Any(x => x is AllowAPIAttribute);

        private readonly List<string> _commandMap = new();

        private readonly InteractionService _service;

        public Func<InteractionProperties, bool> Predicate { get; private set; }

        public char Filter { get; set; } = ':';

        public ApiController(InteractionService service)
        {
            _service = service;
            Populate();

            Predicate = x =>
            {
                var name = string.IsNullOrEmpty(x.Name)
                    ? x.CustomId.Split(Filter)[0]
                    : x.Name;

                if (_commandMap.Any(c => c == name))
                    return true;

                return false;
            };
        }

        public void Populate()
        {
            var predicate = _fetchAttributeFunc.Compile();

            foreach(var command in _service.SlashCommands)
                if (predicate(command))
                    _commandMap.Add(command.Name);

            foreach (var command in _service.ContextCommands)
                if (predicate(command))
                    _commandMap.Add(command.Name);

            foreach (var component in _service.ComponentCommands)
                if (predicate(component))
                {
                    var cid = component.Name;
                    if (cid.Contains('*'))
                        _commandMap.Add(cid.Split(':')[0]);
                    else
                        _commandMap.Add(cid);
                }

            foreach(var modal in _service.ModalCommands)
                if (predicate(modal))
                {
                    var cid = modal.Name;
                    if (cid.Contains('*'))
                        _commandMap.Add(cid.Split(':')[0]);
                    else
                        _commandMap.Add(cid);
                }
        }
    }
}
