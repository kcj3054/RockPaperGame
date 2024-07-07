## 설명
- 초기에 supersocket과 같은 framework를 사용하지 않고 학습을 위해 c# 기본 socket을 사용하지 않으면서 방 게임을 만들었습니다.

- 단순 방을 생성 후 1 : 1 대전이 아닌 m : n 대전이 가능하며, 관전자가 다음 게임에 참가하여 함께 게임을 즐길 수 있습니다.

## 기술 

- c# 

- protobuf

- Mysql (추가 예정)

- Redis (추가 예정)

## 기능

- [x] 로그인 기능 
  - asp.net core (web server) 사용
  - redis에 token 저장 
- [x] 방 생성

- [x] 관전자 기능

- [x] n : m 대전 

- [ ] 계정 설정 기능 (게임 option)

- [ ] 랭킹 기능
  - redis 사용 

- [ ] 친추 추가 기능 
- [ ] 패킷 암호화
  - 공개키 암호화 c# 라이브러리 (System.Security.Cryptography 사용 예정)

- [ ] 중개 서버 추가
  - 패킷 릴레이, 암호화, 하트비트 체크 