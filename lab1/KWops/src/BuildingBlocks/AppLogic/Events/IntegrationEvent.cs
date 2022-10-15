using Newtonsoft.Json;

namespace AppLogic.Events
{
    public class IntegrationEvent
    {
        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }
        
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
        }

    }
}