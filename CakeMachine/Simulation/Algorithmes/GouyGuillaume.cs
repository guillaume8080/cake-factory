using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CakeMachine.Fabrication.ContexteProduction;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;

namespace CakeMachine.Simulation.Algorithmes;

internal class GouyGuillaume: Algorithme
{
    public override bool SupportsSync => true;
    


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
                        //var a = lotsGateauxCuits1.ToArray()[j];
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