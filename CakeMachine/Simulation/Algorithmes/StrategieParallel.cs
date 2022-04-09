using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CakeMachine.Fabrication.ContexteProduction;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;

namespace CakeMachine.Simulation.Algorithmes;

internal class StrategieParallel: Algorithme
{
    public override bool SupportsSync => false;
    public override bool SupportsAsync => true;


    public override void ConfigurerUsine(IConfigurationUsine builder)
    {
        base.ConfigurerUsine(builder);
        builder.NombreEmballeuses = 2;
        builder.NombrePréparateurs = 2;
        builder.NombreFours = 2;


    }

    public override IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
    {
        var postePréparation1 = usine.Préparateurs.First();
        var postePréparation2 = usine.Préparateurs.Last();

        var posteEmballage1 = usine.Emballeuses.First();
        var posteEmballage2 = usine.Emballeuses.Last();
        
        var posteCuisson1 = usine.Fours.First();
        var posteCuisson2 = usine.Fours.Last();
        
        // TODO : ne pas tomber dans l exception de creation de plats
        var z = usine.StockInfiniPlats;
        
        // usine.OrganisationUsine.NombrePréparateurs

        IEnumerable<Plat> plats = null;
        List<List<Plat>> listList = null;
        ParallelQuery<GâteauCru> lotGateauxCrus1 = null;
        ParallelQuery<GâteauCru> lotGateauxCrus2 = null;
        // List<GâteauCru> listOfGâteauCrus = null;
        ParallelQuery<GâteauCuit> lotsGateauxCuits1 = null;
        ParallelQuery<GâteauCuit> lotsGateauxCuits2 = null;
        
        ParallelQuery<GâteauEmballé> lotsGateauxEmballés1 = null;
        ParallelQuery<GâteauEmballé> lotsGateauxEmballés2 = null;

        ParallelQuery<GâteauEmballé> lotsGateauxEmballésX = null;
        List<GâteauEmballé> lotsGateauxEmballés = new List<GâteauEmballé>();

        while (!token.IsCancellationRequested)
        {
            //var plats = Enumerable.Range(0, 10).Select(_ => new Plat());
            
            IEnumerable<Plat[]> platsChunked = usine.StockInfiniPlats.Chunk(10);
            foreach (var item in platsChunked)
            {
                plats = item;
                if (plats != null)
                {
                    break;
                }
            }
            
            
           
            /* var queue = new Queue<Plat>(enumPLat);*/
            listList = ManipulerPréparation(plats, 5);
            // var platsX = plats.Chunk(2);
            //Enumerable<string> m_oEnum = new IEnumerable<string>() { "1", "2", "3"};
            for (int i = 0; i < 2; i++)
            {
                List<Plat>listeCourante = listList[i];
                if (i == 0)
                {
                    lotGateauxCrus1 = listeCourante.AsParallel().Select(_ => postePréparation1.Préparer(_));    
                }
                
                if (i > 0)
                {
                    lotGateauxCrus2 = listeCourante.AsParallel().Select(_ => postePréparation2.Préparer(_));
                }
               
                
                
            }
            
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    lotsGateauxCuits1 = posteCuisson1.Cuire(lotGateauxCrus1.ToArray()).AsParallel();
                }
                
                if (i > 0)
                {
                    lotsGateauxCuits2 = posteCuisson2.Cuire(lotGateauxCrus2.ToArray()).AsParallel();
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j < lotsGateauxCuits1.ToArray().Length; j ++)
                    {
                        var x = posteEmballage1.Emballer(lotsGateauxCuits1.ToArray()[j]);
                        //.Append
                        lotsGateauxEmballés.Add(x);
                    }    
                }
                

                if (i > 0)
                {
                    for (int j = 0; j < lotsGateauxCuits2.ToArray().Length; j ++)
                    {
                        var x = posteEmballage2.Emballer(lotsGateauxCuits2.ToArray()[j]);
                        lotsGateauxEmballés.Add(x);
                    }    
                }
                
            }
           
       
            //
            foreach (var gâteauEmballé in lotsGateauxEmballés)
                yield return gâteauEmballé;
        }
    }
    
    public override async IAsyncEnumerable<GâteauEmballé> ProduireAsync(
        Usine usine,
        [EnumeratorCancellation] CancellationToken token)
    {
        IEnumerable<Plat> plats = null;
        List<List<Plat>> listList = null;
        Préparation postePreparation1 = usine.Préparateurs.First();
        var postePreparation2 = usine.Préparateurs.Last();
        Cuisson posteCuisson = usine.Fours.First();
        Emballage posteEmballage1 = usine.Emballeuses.First();
        Emballage posteEmballage2 = usine.Emballeuses.Last();
        ParallelQuery<Task<GâteauCru>> preparationParallele1 = null;
        ParallelQuery<Task<GâteauCru>>? preparationParallele2 = null;
        List<GâteauCru> lotsGateauxCrus = new List<GâteauCru>();
        List<GâteauCuit> lotsGateauxCuits = new List<GâteauCuit>();
        List<GâteauCuit> lotsGateauxCuits1 = new List<GâteauCuit>();
        List<GâteauCuit> lotsGateauxCuits2 = new List<GâteauCuit>();
        GâteauCuit[] array = null;
        ParallelQuery<Task<GâteauCuit[]>> CollectionAsynchroneGateauxCuits = null;
        IEnumerable<Task<GâteauEmballé>> tacheEmballage1 = null;
        IEnumerable<Task<GâteauEmballé>> tacheEmballage2 = null;
        List<GâteauEmballé> listGateauxEmballes = new List<GâteauEmballé>();
        
        
        
        




        while (!token.IsCancellationRequested)
        {
            IEnumerable<Plat[]> platsChunked = usine.StockInfiniPlats.Chunk(10);
            foreach (var item in platsChunked)
            {
                plats = item;
                if (plats != null)
                {
                    break;
                }
            }
            listList  = ManipulerPréparation(plats, 5);
            for (int i = 0; i < 2; i++)
            {
                List<Plat>listeCourante = listList[i];
                if (i < 1)
                {
                    preparationParallele1 = listeCourante.AsParallel().Select(_ => postePreparation1.PréparerAsync(_));    
                }
                
                if (i > 0)
                {
                   
                    preparationParallele2 = listeCourante.AsParallel().Select(_ => postePreparation2.PréparerAsync(_));
                }
               
                
                
            }
            // // Ces deux lignes sont l'exemple de ce qu'ilne faut pas faire:
            // Mettre en commun des collections dependant de traitement asynchrone
            foreach (var item in preparationParallele1)
            {
                lotsGateauxCrus.Add(await item);
            }

            foreach (var item in preparationParallele2)
            {
                lotsGateauxCrus.Add(await item);
            }

            
            CollectionAsynchroneGateauxCuits  = lotsGateauxCrus.AsParallel().Select(_ => posteCuisson.CuireAsync(_));
           

            foreach (var item in CollectionAsynchroneGateauxCuits)
            {
                //var toto = await await  Task.WhenAny(item);
                
                var oneQuery = await item;
                foreach (var myCake in oneQuery)
                {
                    lotsGateauxCuits.Add(myCake);
                }

            }
            // var x = posteEmballage1.Emballer(lotsGateauxCuits1.ToArray()[j]);
            for (int i = 0; i < lotsGateauxCuits.Count ; i++)
            {
                if (i < lotsGateauxCuits.Count / 2)
                {
                    lotsGateauxCuits1.Add(lotsGateauxCuits.ElementAt(i));
                }

                if (i > lotsGateauxCuits.Count / 2)
                {
                    lotsGateauxCuits2.Add(lotsGateauxCuits.ElementAt(i));
                }
            }

            /*for (int i = 0; i < lotsGateauxCuits1.Count ; i++)
            {
                tacheEmballage1 =  posteEmballage1.EmballerAsync(lotsGateauxCuits1.ElementAt(i));
                tacheEmballage2 = posteEmballage2.EmballerAsync(lotsGateauxCuits2.ElementAt(i));
                
                /*var z = await await Task.WhenAny(x , y);
                yield return z;
            }*/
            //lotsGateauxCrus.AsParallel().Select(_ => posteCuisson.CuireAsync(_));
             tacheEmballage1 = lotsGateauxCuits1.Select(_ => posteEmballage1.EmballerAsync(_));
             tacheEmballage2 = lotsGateauxCuits2.Select(_ => posteEmballage2.EmballerAsync(_));
            var arrayOfEmballeGateaux1 = await Task.WhenAll(tacheEmballage1);
            var arrayOfEmballeGateaux2 = await Task.WhenAll(tacheEmballage2);
            listGateauxEmballes.AddRange(arrayOfEmballeGateaux1);
            listGateauxEmballes.AddRange(arrayOfEmballeGateaux2);


            //var tabGateauxEmballes = await  Task.WhenAll(tacheEmballage1, tacheEmballage2);
            foreach (var gateauEmballe in listGateauxEmballes)
            {
                yield return gateauEmballe;
            }




        }
    }
    
    
    public List<List<Plat>> ManipulerPréparation(IEnumerable<Plat> enumPLat , ushort diviseur)
    {
        var queue = new Queue<Plat>(enumPLat);
        var myCount = queue.Count;
        diviseur = 2;
        List<List<Plat>> listOfListToReturn = new List<List<Plat>>();
        for (int i = 0; i < diviseur; i++)
        {
            List<Plat> maListe = new List<Plat>();
            for (int j = 0; j < myCount/diviseur; j++)
            {
                Plat monPlat = queue.Dequeue();
                maListe.Add(monPlat);
               
            }
            listOfListToReturn.Add(maListe);
        }

        return listOfListToReturn;
    }
    
    
    
   
    
    

    
    
}