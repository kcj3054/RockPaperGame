## 설명

- 단순 방을 생성 후 1 : 1 대전이 아닌 m : n 대전이 가능하며, 관전자가 다음 게임에 참가하여 함께 게임을 즐길 수 있습니다.

### APIServer

[APIServer](./APIServer)


- asp.net core를 이용한 api server

- Login 인증 기능

- 우편 기능 

- 일반적으로 로그인서버, 로비서버라고도 부른다.

### GateWayServer

[GateWayServer](./GatewayServer)

- 프록시 서버로 중간 매개인 gatewayserver를 생성했습니다.

- 기능으로는 추후에 암호화기능을 추가할 예정 

- 현재는 supersocket 기반으로 구현되어있습니다.

- 직렬화 기능으로는 MessagePack을 사용

### GameServer

[GameServer](./GameServer)

- c# socket을 이용하여 구현 

- 다중 가위 바위 보 게임을 진행하는 게임서버 

- 직렬화 기능으로는 protobuf를 사용 

## DBSchema

- [DB 문서 ](./APIServer/SCHEMA.md)
  - USER, ACCOUNT, MAIL, MAIL_REWARD.. 

- REPOSITORY를 옮기면서 초기 스키마가 손실 되어 추가 작성 중  

## 기술 

- c# 
  - .net core 8.0

- protobuf
  - GameServer 직렬화용으로 사용 
- MessagePack
  - GatewayServer에서 사용 

- Mysql
  - RDBMS로 MYSQL을 사용하고 Dapper를 사용 함 
  - 현재 에드 훅 쿼리로 작성 중인 것을 Stored Procedure로 변경 작업 중 
- Redis
  
## 기능

- [x] 로그인 기능 
  - asp.net core (web server) 사용
  - redis에 token 저장 
- [x] 방 생성

- [x] 관전자 기능

- [x] n : m 대전 

- [x] 우편 기능
  - [x] 우편 삭제 
  - [x] 전체 우편 조회

- [ ] Timer를 이용한 가위, 바위, 보 공격 시간 제한 

- [ ] 계정 설정 기능 (게임 option)

- [ ] 랭킹 기능
  - redis 사용 

- [ ] 친추 추가 기능 
- [ ] 패킷 암호화
  - 공개키 암호화 c# 라이브러리 (System.Security.Cryptography 사용 예정)

- [x] 중개 서버 
- 4추가
  - 패킷 릴레이, 암호화, 하트비트 체크 


## 스레드 전략 

- SOCKET에도 DB를 연동하게된다면 DB 스레드는 따로 처리한다.

- IO를 담당하는 스레드와 Packet을 처리하는 스레드도 구분.

- 스레드 수는 기본적으로 CORE * 2 + 1로 가지만, C#의 스레드 풀도 자체적으로 잘 되어있음 

- 방 게임일 경우 로직 스레드 분배 전략은 예시로 1 ~ 100을 담당하는 스레드 101 ~ 200, 201 ~ 300 각 방을 담당하는 스레드들을 분리해서 사용한다.

- mmo 같은 경우는 채널방식을 많이 사용하기도하는데 방게임에도 적용해볼 수 있다.
  - 하나의서버에 여러 채널이 존재하며 한 채널당 패킷처리 스레드를 따로 줄 수도있다. 
