## 계정 정보 
```mysql
create table account
(
    
);
```

## 유저 
- 하나의 account안에 여러 user들이 생성 될 수 있다.
- LoginServer에서 로그인하는 것은 AccountTable, 만들어진 캐릭터 자체들은 userTable
- 랭킹이 필요할시 랭킹 테이블도 생성하면된다 or 칼럼으로 점수 칼럼들을 생성
```mysql
create table user
(
  'userID' INT NOT NULL AUTO_INCREMENT '유저아이디',
   'accounID' BIGINT NOT NULL COMMENT '계정아이디',
   'userName' varchar(20) not null comment '계정이름',
    'createTime' datetime not null comment  '생성일시',
    'deletedTime' datetime  comment '삭제일시'
);
```


## 메일 함 
- user와 mailbox는 1 대 다의 관계 (mailBox에 외래키를 설정)
- user table의 하나의 userID는 mailbox의 여러 레코드와 대응 될 수 있다.
- 외래키를 설정하거나 복합 unique key설정 가능
- 하나의 유저가 여러개의 우편을 받을 수 있다.

- rewardid 누락했음..
```mysql
create table mailbox
(
    'mailID'  int not null auto_increment comment '우편 고유 번호',
    'userID' int not null comment  '유저 식별 아이디'
    'mailTile' varchar(20) not null comment '우편 이름',
    'createTime' datetime   not null comment '우편 생성 일시',
    'expiredTime' datetime not null comment '우편 만료 일시',
    'isReceived' tinyint  not null default 0 '수령 유무',
    primary key (mailID)
);

ALTER TABLE mailbox
add constraint fk_mailbox_uid_user_uid
foreign key (userID) references user(userID)
```

## 메일 보상 
- 메일에 대한 보상테이블 
- 메일함과 메일보상 테이블 또한 1 대 다관게이다.
- 메일 보상은 .. 기획데이터에 정의된 Reward 데이터를 따르긴한다.
- 메일 아이템 삭제 기간도 존재
  - 우편 삭제와 다르게, 우편으로 지급된 이벤트성 아이템들의 기한도 존재.
- 해당 테이블도 기한이 필요하다.(아이템 만을 위한)
```mysql
create table mailReward
(
    'mailID' int not null comment '우편 고유 번호',
    'rewardItem' varchar(20) not null comment '보상 아이템.. 아이템이라고 놓아도되고, 아이템 id를 넣어도된다',
    'rewardCount' int not null comment '보상 수',
    primary key (mailID, rewardItem)
);
```
 