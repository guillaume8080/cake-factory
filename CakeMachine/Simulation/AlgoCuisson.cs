using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal class AlgoCuisson : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsSync => true;
        GâteauCru[] arrayOfGateauCru = new GâteauCru[] {};
        private GâteauCuit[] arrayOfGateauCuits = new GâteauCuit[] {};
        private GâteauEmballé[] aOfGEmballe = new GâteauEmballé[] {};
        //T[] array = new T[] {}
        

        /// <inheritdoc />
        public override IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                
                
                // Cuisson._nbDePlaces == 5
                for (int i = 0; i < 5; i++)
                {
                    
                   
                    var plat = new Plat();

                    var gâteauCru = usine.Préparateurs.First().Préparer(plat);
                    arrayOfGateauCru.Append(gâteauCru);
                    
                    
                    

                    if (i == 4)
                    {
                        Console.WriteLine("nombre de places dans le four atteint");
                        
                    }
                    // yield return gâteauEmballé;
                }

                for (int j = 0; j < arrayOfGateauCru.Length; j++)
                {
                    // // Cuisson.cuire

                    var unGateauCuit = usine.Fours.First().Cuire(arrayOfGateauCru).Single();
                    arrayOfGateauCuits.Append(unGateauCuit);
                }

                for (int k = 0; k < arrayOfGateauCuits.Length; k++)
                {
                    var gâteauEmballé = usine.Emballeuses.First().Emballer(arrayOfGateauCuits[k]);
                    aOfGEmballe.Append(gâteauEmballé);

                }

                for (int l = 0; l < aOfGEmballe.Length; l++)
                {
                    yield return aOfGEmballe[l];
                }





            }
        }

    }
}