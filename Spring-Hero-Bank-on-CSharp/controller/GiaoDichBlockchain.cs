using System;
using Spring_Hero_Bank_on_CSharp;
using Spring_Hero_Bank_on_CSharp.entity;
using Spring_Hero_Bank_on_CSharp.model;

namespace ConsoleApp1
{
    public class GiaoDichBlockchain : GiaoDich
    {
        private static BlockchainAddressModel blockchainAddressModel;
        
        public GiaoDichBlockchain()
        {
            blockchainAddressModel = new BlockchainAddressModel();
        }
        public void RutTien()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành rút tiền tại ví điện tử Blockchain.");
                Console.WriteLine("Vui lòng nhập số tiền cần rút.");
                var amount = double.Parse(Console.ReadLine());
                if (amount > Program.currentLoggedInAddress.Balance)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var blockchainTransaction = new BlockchainTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Type = BlockchainTransaction.TransactionType.WITHDRAW,
                    Amount = amount,
                    Message = "Tiến hành rút tiền tại ví điện tử Blockchain với số tiền: " + amount,
                    CreateAtMlS = DateTime.Now.Ticks,
                    UpdateAtMlS = DateTime.Now.Ticks,
                    Status = BlockchainTransaction.TransactionStatus.DONE
                };
                if (blockchainAddressModel.UpdateBalanceBlockchain(Program.currentLoggedInAddress,blockchainTransaction))
                {
                    Console.WriteLine("Giao dịch thành công.");  
                }
            }
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }

        public void GuiTien()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành gửi tiền tại ví điện tử Blockchain.");
                Console.WriteLine("Vui lòng nhập số tiền cần gửi.");
                var amount = double.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var blockchainTransaction = new BlockchainTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Type = BlockchainTransaction.TransactionType.DEPOSIT,
                    Amount = amount,
                    Message = "Tiến hành gửi tiền tại ví điện tử Blockchain với số tiền: " + amount,
                    CreateAtMlS = DateTime.Now.Ticks,
                    UpdateAtMlS = DateTime.Now.Ticks,
                    Status = BlockchainTransaction.TransactionStatus.DONE
                };
                if (blockchainAddressModel.UpdateBalanceBlockchain(Program.currentLoggedInAddress,blockchainTransaction))
                {
                    
                    Console.WriteLine("Giao dịch thành công.");  
                }
            }
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }

        public void ChuyenTien()
        {
            throw new System.NotImplementedException();
        }

        public void Login()
        {
            Program.currentLoggedInAddress = null;
            Console.Clear();
            Console.WriteLine("Tiến hành đăng nhập hệ thống Blockchain.");
            Console.WriteLine("Vui lòng nhập địa chỉ đăng nhập: ");
            var address = Console.ReadLine();
            Console.WriteLine("Vui lòng nhập private key: ");
            var privateKey = Console.ReadLine();
            var blockchainAddress = blockchainAddressModel.FindByAddressAndPrivateKey(address, privateKey);
            if (blockchainAddress == null)
            {
                Console.WriteLine("Sai địa chỉ tài khoản, vui lòng đăng nhập lại.");
                Console.WriteLine("Ấn phím bất kỳ để tiếp tục.");
                Console.ReadLine();
                return;
            }

            // trong trường hợp trả về khác null.
            // set giá trị vào biến CurrentLoggedInAddress.
            Program.currentLoggedInAddress = blockchainAddress;

        }
    }
}