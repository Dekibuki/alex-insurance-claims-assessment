using Claims.Domain.Models.Audit;
using System.Threading.Channels;

namespace Claims.Infrastructure.Services.Audit
{
    public class AuditQueue
    {
        private readonly Channel<AuditMessage> _channel = Channel.CreateUnbounded<AuditMessage>();

        public ChannelReader<AuditMessage> Reader => _channel.Reader;

        public ValueTask EnqueueAsync(AuditMessage message, CancellationToken ct = default)
        {
            return _channel.Writer.WriteAsync(message, ct);
        }
    }
}
