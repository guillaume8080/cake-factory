using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CakeMachine.Fabrication.ContexteProduction;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;

namespace CakeMachine.Simulation.Algorithmes;

internal class GouyGuillaumeEnConstruction: Algorithme
{
    public override bool SupportsSync => false;
    

    public override void ConfigurerUsine(IConfigurationUsine builder)
    {
        base.ConfigurerUsine(builder);
        builder.NombreEmballeuses = 10;
        builder.NombrePréparateurs = 10;
        builder.NombreFours = 1;


    }

    
    public override IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
    {
        // variables to keep sur
        var posteCuisson = usine.Fours.First();
        List<Préparation> postesPreparation = manipulerTousLesPostesDePreparation(usine.Préparateurs);
        List<Emballage> postesEmballages = ManipulerTousLesPostesDEmballages(usine.Emballeuses);
        IEnumerable<Plat[]> platsChunked = null;
        IEnumerable<Plat> plats = null;
        List<List<Plat>> listList = null;
        List<ParallelQuery<GâteauCru>> lotsGateauxCrus = new List<ParallelQuery<GâteauCru>>();
        // LOts DE gateaux cuits
        List<ParallelQuery<GâteauCuit>> lotsDeGateauxCuits = new List<ParallelQuery<GâteauCuit>>();
        Emballage posteEmballageCourant = null;
        ParallelQuery<GâteauCuit> parallelQueryDeGateauxCourante = null;
        List<GâteauEmballé> listeCakeToYeld = null;
        
        
        //traitement to keep
        while (!token.IsCancellationRequested)
        {
            platsChunked = usine.StockInfiniPlats.Chunk(50);
            foreach (var item in platsChunked)
            {
                plats = item;
                if (plats != null)
                {
                    break;
                }
            }
            // TODO : verifier que cette liste contient bien 10 listes
            listList = ManipulerPréparation(plats);
            
            for (int i = 0; i < 10; i++)
            {
                List<Plat>listeCourante = listList[i];
                Préparation postePrepaCourant = postesPreparation[i];
                
                //TODO: verifier que cette variable contient bel et bien le lot
                
                lotsGateauxCrus.Add(listeCourante.AsParallel().Select(_ => postePrepaCourant.Préparer(_)));
                
            }

            for (int i = 0; i < lotsGateauxCrus.Count; i++)
            {
                var lotsDeCingGateaux = lotsGateauxCrus[i].ToArray();
                var lotsDeCingGateauxCuits = posteCuisson.Cuire(lotsDeCingGateaux).AsParallel();
                lotsDeGateauxCuits.Add(lotsDeCingGateauxCuits);
            }
            
            for (int i = 0; i < postesEmballages.Count; i++)
            { 
                posteEmballageCourant = postesEmballages[i];
                parallelQueryDeGateauxCourante = lotsDeGateauxCuits[i];

                for (int j = 0; j < 5; j++)
                {
                    var gâteauEmballé = posteEmballageCourant.Emballer(parallelQueryDeGateauxCourante.ToArray()[j]);
                    listeCakeToYeld.Add(gâteauEmballé);
                }
                
            }

            
            foreach (var gâteauEmballé in listeCakeToYeld)
                yield return gâteauEmballé;
        }
    }
    
  
    public List<List<Plat>> ManipulerPréparation(IEnumerable<Plat> enumPLat )
    {
        var queue = new Queue<Plat>(enumPLat);
        var myCount = queue.Count;
        ushort diviseur = 10;
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
    // Oui , je n'arrive pas à manipuler un objet sans son type.
    public List<Préparation> manipulerTousLesPostesDePreparation(IEnumerable<Préparation> postesDePreparation)
    {
        var queue = new Queue<Préparation>(postesDePreparation);
        List<Préparation> listeToReturn = new List<Préparation>();
        for (int i = 0; i < queue.Count(); i++)
        {
            Préparation poste = queue.Dequeue();
            listeToReturn.Add(poste);
        }

        return listeToReturn;
    }
    public List<Emballage> ManipulerTousLesPostesDEmballages(IEnumerable<Emballage> postesDeEmballage)
    {
        var queue = new Queue<Emballage>(postesDeEmballage);
        List<Emballage> listeToReturn = new List<Emballage>();
        for (int i = 0; i < queue.Count(); i++)
        {
            Emballage poste = queue.Dequeue();
            listeToReturn.Add(poste);
        }

        return listeToReturn;
    }
    
    
   
    
    

    
    
}