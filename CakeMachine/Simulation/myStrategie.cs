using System.Runtime.CompilerServices;
using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;
using CakeMachine.Utils;

namespace CakeMachine.Simulation
{
    public class myStrategie : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsSync => false;

        /// <inheritdoc />
        public override bool SupportsAsync => true;
        
        // Cette strategie prend base sur le fait que Cuire est le plus rapide
        // Il nous faut donc un minimum de ressources pour Cuire
        
        // Garbage Collector

        public override void ConfigurerUsine(IConfigurationUsine builder)
        {
             base.ConfigurerUsine(builder);
            // builder.
        }

        /// <inheritdoc />
        public override async IAsyncEnumerable<GâteauEmballé> ProduireAsync(Usine usine, [EnumeratorCancellation] CancellationToken token)
        {
            var capacitéFour = usine.OrganisationUsine.ParamètresCuisson.NombrePlaces;

            var postePréparation = usine.Préparateurs.Single();
            var posteEmballage = usine.Emballeuses.Single();
            var posteCuisson = usine.Fours.Single();

            while (!token.IsCancellationRequested)
            {
                var plats = Enumerable.Range(0, 10).Select(_ => new Plat());

                var gâteauxCrus = plats
                    .Select(postePréparation.PréparerAsync)
                    .EnumerateCompleted();
                
                var xCrus = new List<GâteauCru>();
                await foreach (var gâteauCru in gâteauxCrus)
                {
                    xCrus.Add(gâteauCru);
                }

                CuireOuNon(xCrus);

                var gâteauxCuits = CuireParLotsAsync(gâteauxCrus, posteCuisson, capacitéFour);

                var tâchesEmballage = new List<Task<GâteauEmballé>>();
                await foreach(var gâteauCuit in gâteauxCuits.WithCancellation(token))
                    tâchesEmballage.Add(posteEmballage.EmballerAsync(gâteauCuit));

                await foreach (var gâteauEmballé in tâchesEmballage.EnumerateCompleted().WithCancellation(token))
                    yield return gâteauEmballé;
            }
        }

        private static  Boolean CuireOuNon(List<GâteauCru> gâteaux)
        {
            var countOfGateauxCrus = 0;
             foreach (var aCake in gâteaux)
            {
                countOfGateauxCrus ++;
            }

             if (countOfGateauxCrus != 100)
             {
                 return true;
             }

             return false;
        }

        private static async IAsyncEnumerable<GâteauCuit> CuireParLotsAsync(
            IAsyncEnumerable<GâteauCru> gâteaux, 
            Cuisson four,
            uint capacitéFour)
        {
            var buffer = new List<GâteauCru>((int) capacitéFour);
            await foreach(var gâteauCru in gâteaux)
            {
                buffer.Add(gâteauCru);

                if (buffer.Count != capacitéFour) continue;

                var gâteauxCuits = await four.CuireAsync(buffer.ToArray());
                foreach (var gâteauCuit in gâteauxCuits)
                    yield return gâteauCuit;

                buffer.Clear();
            }
        }
    }
}
