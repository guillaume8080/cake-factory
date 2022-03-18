using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal class AlgoCuisson : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsSync => true;
        GâteauCru[] arrayOfGateauCru = new GâteauCru[] {};
        private GâteauCuit[] arrayOfGateauCuits = new GâteauCuit[0] {};
        private GâteauEmballé[] aOfGEmballe = new GâteauEmballé[] {};


        private List<GâteauCru> listOfGateauCru = new();
        private List<GâteauCuit> listOfGateauCuit = new();

        private List<GâteauEmballé> ListGatEmballe = new();
        //T[] array = new T[] {}
        

        /// <inheritdoc />
        public override IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                
                // usine.Fours.First().Cuire()
                
                for (int i = 1; i < 5; i++)
                {
                    
                   
                    var plat = new Plat();

                    var gâteauCru = usine.Préparateurs.First().Préparer(plat);
                    listOfGateauCru.Add(gâteauCru);
                    
                    
                }
                
                // Cette methode n est pas correcte , on appelle non pas la mehode Cuire avec un gateau mais un tabelau
                // Cuisson._nbDePlaces == 5
                for (int j = 0; j < listOfGateauCru.Count; j++)
                {
                    // // Cuisson.cuire
                    // GâteauCru[] arrayOfGateauCru = new GâteauCru[listOfGateauCru.Count] {};
                    
                    var unGateauCuit = usine.Fours.First().Cuire(listOfGateauCru.ElementAt(j)).Single();
                    listOfGateauCuit.Add(unGateauCuit);
                }

                for (int k = 0; k < listOfGateauCuit.Count; k++)
                {
                    var gâteauEmballé = usine.Emballeuses.First().Emballer(listOfGateauCuit.ElementAt(k));
                    ListGatEmballe.Add(gâteauEmballé);

                }

                for (int l = 0; l < ListGatEmballe.Count; l++)
                {
                    yield return ListGatEmballe.ElementAt(l);
                }





            }
        }

    }
}