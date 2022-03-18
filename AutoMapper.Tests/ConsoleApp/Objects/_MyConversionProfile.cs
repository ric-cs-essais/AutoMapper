using AutoMapper;


namespace ConsoleApp
{
    //Un Profile permet de définir un Profil de mappage, qui pourra ensuite être appliqué à un type MapperConfiguration, via une méthode AddProfile.
    public class MyConversionSource1Desti1Profile : Profile
    {
        public MyConversionSource1Desti1Profile()
        {
            this._createBindings();
        }

        private void _createBindings()
        {
            CreateMap<Source1, Desti1>()
                .ForMember(poDesti => poDesti.AutreValeur, opt => opt.MapFrom(poSource => 20 * poSource.autreValeur))
                .ForMember(poDesti => poDesti.Autre_Valeur2, opt => opt.MapFrom(poSource => 100 * poSource.autreValeur2));
        }

    }

}
