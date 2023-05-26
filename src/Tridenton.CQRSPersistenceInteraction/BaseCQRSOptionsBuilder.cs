using Tridenton.Core.Models;

namespace Tridenton.CQRSPersistenceInteraction;

public abstract class BaseCQRSOptionsBuilder : ResponseOptionsBuilder
{
    private Type? _dataContextType;
    public Type? DataContextType
    {
        get => _dataContextType;
        set
        {
            if (_dataContextType is null)
            {
                _dataContextType = value;
            }
        }
    }

    private Type _requestLogEntityType = typeof(RequestLog);
    public Type RequestLogEntityType
    {
        get => _requestLogEntityType;
        set
        {
            _requestLogEntityType = value;
        }
    }
}