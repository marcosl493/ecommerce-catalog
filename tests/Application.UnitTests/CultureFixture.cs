using System.Globalization;

namespace Application.UnitTests;

public class CultureFixture
{
    public CultureFixture()
    {
        var culture = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}

[CollectionDefinition("Culture collection")]
public class CultureCollection : ICollectionFixture<CultureFixture> { }
