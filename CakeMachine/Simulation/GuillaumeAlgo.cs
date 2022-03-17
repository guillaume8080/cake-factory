using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal class GuillaumeAlgo : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsSync => true;

        private List<GâteauEmballé> mesGateaux = new List<GâteauEmballé>();
        /// <inheritdoc />
        public override IEnumerable<CakeMachine.Fabrication.Elements.GâteauEmballé> Produire(Usine usine, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                for (int i = 0; i < 1; i++)
                {
                    var plat = new Plat();

                    var gâteauCru = usine.Préparateurs.First().Préparer(plat);
                    var gâteauCuit = usine.Fours.First().Cuire(gâteauCru).Single();
                    var gâteauEmballé = usine.Emballeuses.First().Emballer(gâteauCuit);
                    
                    mesGateaux.Add(gâteauEmballé);

                    for (int j = 0; j < mesGateaux.Count; j++)
                    {
                        if (j == 2)
                        {
                            yield break;
                        }
                        yield return mesGateaux[j];
                       
                    }

                }
                var  enumOfGateaux = (IEnumerable<CakeMachine.Fabrication.Elements.GâteauEmballé>)mesGateaux ;
               

            }
        }

       
        

//AlgoCuisson


        }
}