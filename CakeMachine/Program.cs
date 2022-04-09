using System.Runtime.CompilerServices;
using CakeMachine.Simulation;

[assembly:InternalsVisibleTo("CakeMachine.Test")]

const int nombreGâteaux = 100;

var runner = new MultipleAlgorithmsRunner();
// await runner.ProduireNGâteaux(nombreGâteaux);
TimeSpan dureeProduction = new TimeSpan(0,0,10);
await runner.ProduirePendant(dureeProduction);