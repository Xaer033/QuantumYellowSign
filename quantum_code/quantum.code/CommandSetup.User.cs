using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Sample;
using Quantum.YellowSign;

namespace Quantum {
  public static partial class DeterministicCommandSetup {
    static partial void AddCommandFactoriesUser(ICollection<IDeterministicCommandFactory> factories, RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
            factories.Add(new CommandSpawnTower());
        }
  }
}
