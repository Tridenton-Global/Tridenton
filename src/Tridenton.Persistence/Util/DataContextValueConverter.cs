using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace Tridenton.Persistence;

public class DataContextValueConverter<TModel, TProvider> : ValueConverter<TModel, TProvider>
{
    public DataContextValueConverter(Expression<Func<TModel, TProvider>> convertToProviderExpression, Expression<Func<TProvider, TModel>> convertFromProviderExpression, ConverterMappingHints? mappingHints = null) : base(convertToProviderExpression, convertFromProviderExpression, mappingHints) { }
}