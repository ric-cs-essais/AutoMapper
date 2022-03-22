using System;

using AutoMapper;

using ConsoleApp._common;



namespace ConsoleApp.Tests_ReverseMap
{
    public class TestsReverseMap
    {
        public static void Run()
        {

            Console.WriteLine("\n===================== ReverseMap ============================");

            conversion1();

            Console.ReadKey();
        }



        private static void conversion1()
        {
            TypeSource oSource;
            TypeDesti oDesti;



            //------------------------ Conversion TypeSource -> TypeDesti ----------------------------
            oSource = new TypeSource
            {
                entierS = 10,
                chaine = "sourceStr10",
                source2 = new TypeSource2
                {
                    entier2 = 20,
                    chaine2 = "source2Str20"
                },

                obj3 = new TypeCommun
                {
                    entier3 = 333,
                    chaine3 = "300"
                }
            };
            Console.WriteLine("\n\n----- TypeSource -> TypeDesti ------\n\n");
            oDesti = _getAutoMapper().Map<TypeDesti>(oSource);
            Console.WriteLine("TypeSource instance (source): "); Debug.Show(oSource);
            Console.WriteLine("TypeDesti instance (desti): "); Debug.Show(oDesti);



            //------------------------ Conversion TypeDesti -> TypeSource ----------------------------
            oDesti = new TypeDesti
            {
                entierD = 60,
                chaine = "destiStr60!!!!!",
                desti2 = new TypeDesti2
                {
                    entier2 = 90,
                    chaine2 = "desti2Str120"
                },

                obj3 = new TypeCommun
                {
                    entier3 = 999,
                    chaine3 = "900"
                }
            };
            //La conversion en sens inverse n'est envisageable(pas de plantage), QUE parce-que dans le paramétrage du IMapper  (dans _getAutoMapper()),
            //on formule explicitement(ForMember, etc...) (ou implicitement via ReverseMap), ce que l'on veut en cas de conversion inverse.
            //ATTENTION : seuls les cas dont l'inversion est explicitement mentionnée ou bien implictement mentionnée mais SIMPLE,
            //seront traités.
            Console.WriteLine("\n\n----- TypeDesti -> TypeSource   (exploitation des regles créées par ReverseMap) ------\n\n");
            oSource = _getAutoMapper().Map<TypeSource>(oDesti);
            Console.WriteLine("TypeDesti instance (source): "); Debug.Show(oDesti);
            Console.WriteLine("TypeSource instance (desti): ");  Debug.Show(oSource);
            Console.WriteLine("\n\n-----------\n\n");

        }

        private static IMapper _getAutoMapper()
        {
            MapperConfiguration oMapperConfig = new MapperConfiguration(
                (IMapperConfigurationExpression poConfig) => {

                    poConfig.CreateMap<TypeSource, TypeDesti>()
                        .ForMember(poDesti => poDesti.entierD, opt => opt.MapFrom(poSource => poSource.entierS)) //Règle simple => ReverseMap pourra ajouter la règle inverse

                        .ForMember(poDesti => poDesti.chaine, opt => opt.MapFrom(poSource => poSource.chaine + "!!!!!")) //<<sera Non pris en compte
                                                                                                                         // par ReverseMap (car trop complexe pour lui)

                        .ForPath(poDesti => poDesti.desti2.entier2, opt => opt.MapFrom(poSource => poSource.source2.entier2)) //Règle simple => ReverseMap pourra ajouter la règle inverse

                        .ReverseMap() //<< N'opérera que pour les ForMember,ForPath,etc..., à mappage direct et SIMPLE !!
                                      //  (comme ici poSource.entierS <---> poDesti.entierD,
                                      //    ou encore  poSource.source2.entier2 <---> poDesti.desti2.entier2)
                    ;

                    //REM. le ReverseMap (en plus de ce qu'il ajoute (voir plus bas)) , ajoute AUSSI ceci :
                    // poConfig.CreateMap<TypeDesti, TypeSource>(); //<<< Cette ligne isolée, à elle toute seule, autorise une conversion inverse (TypeDesti -> TypeSource)
                                                                    //mais sans règle de gestion spécifique (ForMember, ForPath),
                                                                    //c-à-d une conversion dans la mesure de ce qui serait convertible (même nom champ, etc...).


                    //Le ReverseMap ajoute AUSSI des règles (ForMember, ForPath, etc...) de conversion inverses de celles mentionnées
                    ////  (mais UNIQUEMENT celles simples, celles qu'il PEUT inverser simplement)
                    //Le ReverseMap ci-dessus crée donc implicitement les 2 nouvelles règles de conversion ci-dessous
                    //  (uniquement celles-ci car l'autre (ForMember avec +"!!!!!") est trop complexe pour lui),
                    // Notre ReverseMap équivaut donc à rajouter texto ceci (on évite donc d'avoir à écrire) <<< :
                    //poConfig.CreateMap<TypeDesti, TypeSource>()   //TypeDesti -> TypeSource  cette fois
                    //    .ForMember(poSource => poSource.entierS, opt => opt.MapFrom(poDesti => poDesti.entierD)) //Règle inverse (simple)
                    //    .ForPath(poSource => poSource.source2.entier2, opt => opt.MapFrom(poDesti => poDesti.desti2.entier2)) //Règle inverse (simple)
                    //;

                    //Notre IMapper ainsi paramétré sera alors exploitable dans les 2 sens TypeDesti <----> TypeSource,
                    //via donc  oSource=oMapper.Map<TypeSource>(oDesti)  ou à l'inverse via   oDesti=oMapper.Map<TypeDesti>(oSource)

                    //PAR CONTRE comme je le disais, les formes plus évoluées de ForMember, etc..., comme ici 
                    //   .ForMember(poDesti => poDesti.chaine, opt => opt.MapFrom(poSource => poSource.chaine+"!!!!!"))
                    // ne sont pas réversibles pour ReverseMap, ReverseMap les ignore donc, c-à-d ne peut en déduire/obtenir une règle inverse.
                    // Donc pour ce ForMember, si l'on veut tout de même expliciter l'inversion, il faut alors écrire manuellement le ForMember inverse,
                    // à savoir ici, celui qui supprime les 5 "!!!!!" de la fin de la chaîne poDesti.chaine pour obtenir poSource.chaine,
                    // soit la règle manuelle à écrire ici :
                    //  poConfig.CreateMap<TypeDesti, TypeSource>()    //TypeDesti --->TypeSource
                    //    .ForMember(poSource => poSource.chaine, opt => opt.MapFrom(poDesti => poDesti.chaine.Substring(0, poDesti.chaine.Length-5)))
                    //  ;
                }
            );


            IMapper oMapper = oMapperConfig.CreateMapper();
            return (oMapper);
        }


    }




}
