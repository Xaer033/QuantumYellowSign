using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum {
  public static class SystemSetup {
    public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
            return new SystemBase[] {
        // pre-defined core systems
        new Core.CullingSystem3D(),
        new Core.PhysicsSystem3D(),

        Core.DebugCommand.CreateSystem(),

        new Core.EntityPrototypeSystem(),
        new Core.PlayerConnectedSystem(),

        // user systems go here 
        new Tiles.TilePathfinderSystem(),

        new Sample.ItemTileSystem(),
        new Sample.CallbackMoveSystem(),
        new Sample.ClickMoveSystem(),
        new Sample.EditTileMapSystem(),
        new Sample.AIDecisionSystem(),
        
        new TowerSystem(),
      };
    }
  }
}
