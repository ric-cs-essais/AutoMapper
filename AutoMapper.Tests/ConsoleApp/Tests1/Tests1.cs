using System;
using System.Collections.Generic;
using AutoMapper;

using ConsoleApp._common;


namespace ConsoleApp
{
    public class Tests1
    {
        public static void Run()
        {
            conversion1_DirecteSansRetravailler();
            conversion2_conversionParametree();
            conversion3_viaProfile();

            listConversion();

            conversion_afterMap_BeforeMap();

            Console.ReadKey();
        }


        private static Source1 _getSource1()
        {
            Source1 oSource1 = new Source1
            {
                dateTime = DateTime.Parse("31/12/2014 09:34:59"),
                sValeurDirecte = "ValDirecte",
                //memeNomPasMemeType = "xxx", //Décommenter => fait planter car type incompatible entre les 2 classes
                autreValeur = 21,
                autreValeur2 = 150,

                //ValeurDeTypeAConvertir = "p"
                ValeurDeTypeAConvertir = "56",

                oTypeAutre = new TypeAutreSource { sChamp1 = "29" },

                oneDouble1 = 11,
                oneDouble2 = 9,
                oTypeAutre2 = new TypeAutreSource2
                {
                    oneDouble3 = 7
                },

                oneFloat = 8.5f,

                toSomme1 = 5000,
                toSomme2 = 3,

                poids = 51,

                unEntier = 66,
                reponse = true
            };


            return (oSource1);
        }


        //=====================================================================



        private static void conversion1_DirecteSansRetravailler()
        {
            Console.WriteLine("\n\n\n===========  conversion1_DirecteSansRetravailler  =============\n\n");

            Source1 oSource1 = _getSource1();
            Desti1 oDesti1;

            /*
            MapperConfiguration oMapperConfig = new MapperConfiguration(
                poConfig => poConfig.CreateMap<Source1, Desti1>()
            );
            IMapper oMapper = oMapperConfig.CreateMapper();
            oDesti1 = oMapper.Map<Source1, Desti1>(oSource1);
            */

            //Pour la correspondance des champs,
            // il est Insensible à la casse & sait faire correpondre CamelCase <---> PascalCase (champ autreValeur2).
            // PAR CONTRE ATTENTION :
            //    il génère une Exception
            //    en cas de correspondance trouvée(au niveau du nom du champ donc),
            //     ET que ce champ est renseigné dans l'objet source
            //    MAIS que les types (champ source/ champ desti) sont trop incompatibles (c-à-d pas de conversion automatique possible)
            //     Par exemple convertir "p" en int est impossible, par contre "56" en int peut se faire automatiquement (sans .ForMember())


            //Façon plus générique 
            oDesti1 = new Desti1();
            oDesti1 = _conversionDirecteSansRetravailler(oSource1, oDesti1);


            Debug.Show(oDesti1);
        }
        private static TDesti _conversionDirecteSansRetravailler<TSource, TDesti>(TSource poSource, TDesti poDesti)
        {
            poDesti = _getAutoMapper<TSource, TDesti>().Map<TDesti>(poSource);

            return (poDesti);
        }
        private static IMapper _getAutoMapper<TSource, TDesti>()
        {
            MapperConfiguration oMapperConfig = new MapperConfiguration(
                (IMapperConfigurationExpression poConfig) => poConfig.CreateMap<TSource, TDesti>()
            );
            IMapper oMapper = oMapperConfig.CreateMapper();

            return (oMapper);
        }




        //-----------------------------------------------------------

        private static void conversion2_conversionParametree()
        {
            Console.WriteLine("\n\n\n===========  conversion2_conversionParametree  =============\n\n");

            Source1 oSource1 = _getSource1();
            Desti1 oDesti1;

            oDesti1 = _getAutoMapper2<Source1, Desti1>().Map<Desti1>(oSource1);

            Debug.Show(oDesti1);
        }
        private static IMapper _getAutoMapper2<TSource, TDesti>()
            where TSource : Source1
            where TDesti : Desti1
        {
            MapperConfiguration oMapperConfig = new MapperConfiguration(
                (IMapperConfigurationExpression poConfig) => {

                    poConfig.CreateMap<TSource, TDesti>()
                     .ForMember(poDesti => poDesti.AutreValeur, opt => opt.MapFrom(poSource => 2 * poSource.autreValeur))
                     .ForMember(poDesti => poDesti.Autre_Valeur2, opt => opt.MapFrom(poSource => 3 * poSource.autreValeur2))

                     .ForMember(poDesti => poDesti.hour, opt => opt.MapFrom(poSource => poSource.dateTime.Hour))
                     .ForMember(poDesti => poDesti.minutes, opt => opt.MapFrom(poSource => poSource.dateTime.Minute))
                     .ForMember(poDesti => poDesti.seconds, opt => opt.MapFrom(poSource => poSource.dateTime.Second))
                     .ForMember(poDesti => poDesti.seconds, opt => opt.MapFrom(poSource => poSource.dateTime.Second))

                      //.ForMember(poDesti => poDesti.ValeurDeTypeAConvertir, opt => opt.MapFrom(poSource => Int32.Parse(poSource.ValeurDeTypeAConvertir)))

                      .ForPath(poDesti => poDesti.oTypePerso.iChampX,
                             opt => opt.MapFrom(poSource => Int32.Parse(poSource.oTypeAutre.sChamp1) + 2)
                            ) //Utiliser ForPath au lieu de ForMember, lorsqu'on cible un chemin interne
                              //de l'objet Desti.

                      //.ForMember(poDesti => poDesti.maSomme, opt => opt.MapFrom<MyTotal1Resolver<TSource, TDesti>>())  //Utilisation d'un IValueResolver
                      .ForMember(poDesti => poDesti.maSomme, opt => opt.MapFrom(poSource => poSource.toSomme1 + poSource.toSomme2))  //Résultat équivalant à ci-dessus

                    .ForMember(poDesti => poDesti.poids, opt => opt.ConvertUsing(new KilogrammesVersGrammesAsStringConverter()))  //Utilisation d'un IValueConverter (le membre source sera celui matchant par le nom à "poids")
                    //.ForMember(poDesti => poDesti.poids, opt => opt.MapFrom(poSource => $"{1000 * poSource.poids}g"))  //Résultat équivalant à ci-dessus


                    //.ForMember(poDesti => poDesti.poids, opt => opt.ConvertUsing(new KilogrammesVersGrammesAsStringConverter(), poSource => poSource.autreValeur))  //Utilisation d'un IValueConverter,
                    //en précisant le mebre source cette fois
                    //.ForMember(poDesti => poDesti.poids, opt => opt.MapFrom(poSource => $"{1000 * poSource.autreValeur}g"))  //Résultat équivalant à juste ci-dessus
                    ;



                    //Grâce à ci-dessous, pour tout membre(de 1er niveau uniquement) de type double et renseigné
                    //dans la source,
                    //s'il y a correspondance de nom dans la desti. et que ce membre desti. est de type string,
                    //alors l'affecter comme indiqué ci-dessous :
                    poConfig.CreateMap<double, string>()
                      .ConvertUsing(pnDouble => $"-*-{pnDouble + 2000}-*- :)");

                    //Autre façon de procéder pour le ConvertUsing
                    poConfig.CreateMap<float, string>()
                      .ConvertUsing<FloatTypeToString>();


                    poConfig.ValueTransformers.Add<string>(psDestiString => $"oé{psDestiString}éo"); //Applique cette transformation SUPPLEMENTAIRE à tout champ desti de type string ciblé,
                                                                                                     //MAIS l'applique AVANT la première éventuelle conversion (différente de ValueTransformers.Add).
                    poConfig.ValueTransformers.Add<string>(psDestiString => $"'{psDestiString}'");//Applique cette transformation SUPPLEMENTAIRE à tout champ desti de type string ciblé,
                                                                                                  //MAIS l'applique AVANT la première éventuelle conversion (différente de ValueTransformers.Add).



                }
            );
            IMapper oMapper = oMapperConfig.CreateMapper();

            return (oMapper);
        }


        //-------------------------------------------------------------

        private static void conversion3_viaProfile()
        {
            Console.WriteLine("\n\n\n===========  conversion3_viaProfile  =============\n\n");

            Source1 oSource1 = _getSource1();
            Desti1 oDesti1;

            oDesti1 = _getAutoMapper3().Map<Desti1>(oSource1);

            Debug.Show(oDesti1);
        }

        private static IMapper _getAutoMapper3()
        {
            MapperConfiguration oMapperConfig = new MapperConfiguration(
                (IMapperConfigurationExpression poConfig) => poConfig.AddProfile<MyConversionSource1Desti1Profile>()
            );
            IMapper oMapper = oMapperConfig.CreateMapper();

            return (oMapper);
        }



        //=========================================================================================

        private static List<Source2> _getSource2List()
        {
            List<Source2> oSource2List = new List<Source2> {
                new Source2
                {
                    entier = 333,
                    chaineNumerique = "33",
                    date = DateTime.Parse("30/11/2017 07:20:19")
                },
                new Source2
                {
                    entier = 444,
                    chaineNumerique = "44",
                    date = DateTime.Parse("31/12/2018 08:30:29")
                }
            };
            return (oSource2List);
        }

        private static void listConversion()
        {
            Console.WriteLine("\n\n\n===========  listConversion  =============\n\n");

            List<Source2> oSource2List = _getSource2List();

            List<Desti2> oDesti2List = _getListMapper().Map<List<Desti2>>(oSource2List);



            Debug.Show(oDesti2List);
        }

        private static IMapper _getListMapper()
        {
            MapperConfiguration oMapperConfig = new MapperConfiguration(
                (IMapperConfigurationExpression poConfig) => {

                    poConfig.CreateMap<Source2, Desti2>()
                     .ForMember(poDesti => poDesti.date, opt => opt.MapFrom(poSource => poSource.date.ToString() + " !!!"))
                    //Pas besoin de gestion de conversion pour les autres champs, c'est automatique (nombre->string, et string->nombre)
                    ;


                }
            );
            IMapper oMapper = oMapperConfig.CreateMapper();
            return (oMapper);
        }


        //---------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void conversion_afterMap_BeforeMap()
        {
            Console.WriteLine("\n\n===================== BeforeMap, AfterMap ============================\n");

            Source2 oSource2 = new Source2
            {
                entier = 333,
                chaineNumerique = "33",
                date = DateTime.Parse("30/11/2017 07:20:19")
            };

            Desti2 oDesti2 = _getMapperBeforeAfter1().Map<Desti2>(oSource2);
            Debug.Show(oDesti2);

            Desti2 oDesti2b = _getMapperBeforeAfter2().Map<Desti2>(oSource2);
            Debug.Show(oDesti2b);
        }

        private static IMapper _getMapperBeforeAfter1()
        {
            MapperConfiguration oMapperConfig = new MapperConfiguration(
                (IMapperConfigurationExpression poConfig) => {

                    poConfig.CreateMap<Source2, Desti2>()
                     .BeforeMap((poSource, poDesti) =>{
                         poSource.entier += 1000;
                     })
                     .AfterMap((poSource, poDesti) => {
                         poDesti.date += "***";
                     })
                     .ForMember(poDesti => poDesti.date, opt => opt.MapFrom(poSource => poSource.date.ToString() + " !!!"))

                    ;


                }
            );
            IMapper oMapper = oMapperConfig.CreateMapper();
            return (oMapper);
        }

        private static IMapper _getMapperBeforeAfter2()
        {
            MapperConfiguration oMapperConfig = new MapperConfiguration(
                (IMapperConfigurationExpression poConfig) => {

                    poConfig.CreateMap<Source2, Desti2>()
                     .BeforeMap<MyBeforeMapAction>()
                     .AfterMap<MyAfterMapAction>()
                     .ForMember(poDesti => poDesti.date, opt => opt.MapFrom(poSource => poSource.date.ToString() + " !!!"))

                    ;


                }
            );
            IMapper oMapper = oMapperConfig.CreateMapper();
            return (oMapper);
        }




    }



    //=========================================================================================
    //=========================================================================================
    //=========================================================================================
    class FloatTypeToString : ITypeConverter<float, string>
    {
        public string Convert(float pfValeurSource, string dummy, ResolutionContext dummy2)
        {
            return $"-!-{pfValeurSource + 3000}-!- ^^";
        }
    }

    class MyTotal1Resolver<TSource, TDesti> : IValueResolver<TSource, TDesti, int>
        where TSource : Source1
    {
        public int Resolve(TSource source, TDesti destination, int dummy, ResolutionContext dummy2)
        {
            return (source.toSomme1 + source.toSomme2);
        }
    }


    class KilogrammesVersGrammesAsStringConverter : IValueConverter<int, string>
    {
        public string Convert(int correspondingSourceMemberValue, ResolutionContext dummy)
        {
            return ($"{1000 * correspondingSourceMemberValue}g");
        }
    }

    class MyBeforeMapAction : IMappingAction<Source2, Desti2>
    {
        public void Process(Source2 poSource, Desti2 poDesti, ResolutionContext dummy)
        {
            poSource.chaineNumerique += "88";
            poSource.entier += 2000;
        }
    }

    class MyAfterMapAction : IMappingAction<Source2, Desti2>
    {
        public void Process(Source2 poSource, Desti2 poDesti, ResolutionContext dummy)
        {
            poDesti.date += "^^^^^^^";
        }
    }
}
