# 방 입장

```mermaid

sequenceDiagram 
    participant C as Client
    participant GS as GameServer Server
  
    autonumber
    
    C ->> GS:  VerifyPlayerRequest
    GS ->>GS:  ValidatePlayerId
    GS ->> C:  VerifyPlayerResponse
    C ->> GS:  ShowRoomRequest
    GS->>  C:  ShowRoomResponse
    Note right of GS: RoomState(InGame, PreGame) <br/> 이미 진행중인 방에도 입장이 가능하다
    C ->> GS:  EnterRoomRequest
    GS->>  C:  EnterRoomResponse
    Note right of GS: 플레이어가 룸 입장시 순번표 발급 <br/> 입장과 동시에 ready를 할 것인지 아닌지 응답한다.
    Note left of C: UserState(Spectator, Player, Host) <br/>방을 개설한 유저는 자동적으로 방장이된다.
    
````
