// namespace Server;
//
// //TODO : 수정예정 
// //큰 메모리하나 딱 만들어서 거기서 read write를 하면 효율적이다 계속해서 
// public class RecvBuff
// {
//     // 현재 메모리의 커서..
//     // 현재 읽고 (Read하고 있는 부분!)
//     
//     private ArraySegment<byte> _buff;
//     private int readPos = 0;
//     private int wirtePos = 0;
//     
//     public RecvBuff(int buffSize)
//     {
//         _buff = new ArraySegment<byte>(new byte[1024]);
//     }
//     
//     //Write
//     public void Write()
//     {
//         
//     }
//     
//     //Read
//     public void Read()
//     {
//         
//     }
//     
//     //
//     
// }