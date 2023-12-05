import { Room, Client } from "colyseus";
import { GameRoomState, Player } from "./schema/MyRoomState";
export type Position = {
  x: number;
  y: number;
  z: number;
};
export class GameRoom extends Room<GameRoomState> {
  onCreate(options: any) {
    this.setState(new GameRoomState());

    this.onMessage("type", (client, message) => {
      //
      // handle "type" message
      //
    });
  }

  onJoin(client: Client, options: any) {
    console.log(client.sessionId, "joined!");

    this.state.players.set(client.sessionId, new Player());

    // Send welcome message to the client.
    client.send("welcomeMessage", "Welcome to Colyseus!");

    // Listen to position changes from the client.
    this.onMessage("position", (client, position: Position) => {
      console.log(`position x ${position.x}, ${position.y}, ${position.z}`);
      const player = this.state.players.get(client.sessionId);
      player.x += position.x;
      player.y += position.y;
      player.z += position.z;
      console.log(`updated position x ${player.x}, ${player.y}, ${player.z}`);
    });
  }

  onLeave(client: Client, consented: boolean) {
    console.log(client.sessionId, "left!");
  }

  onDispose() {
    console.log("room", this.roomId, "disposing...");
  }
}
