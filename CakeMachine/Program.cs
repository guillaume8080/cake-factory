using System.Runtime.CompilerServices;
using CakeMachine.Simulation;

[assembly:InternalsVisibleTo("CakeMachine.Test")]

const int nombreGâteaux = 100;
const int nombreGateaux2 = 1250;
var runner = new MultipleAlgorithmsRunner();
await runner.ProduireNGâteaux(nombreGateaux2);
//TimeSpan dureeProduction = new TimeSpan(0,1,0);
//await runner.ProduirePendant(dureeProduction);