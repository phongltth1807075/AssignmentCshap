using System;
using MySql.Data.MySqlClient;
using Spring_Hero_Bank_on_CSharp.entity;

namespace Spring_Hero_Bank_on_CSharp.model
{
    public class BlockchainAddressModel
    {
        public BlockchainAddress FindByAddressAndPrivateKey(string address, string privateKey)
        {
            var cmd = new MySqlCommand(
                "select * from BlockChainAccount where address = @address and privateKey = @privateKey",
                ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@privateKey", privateKey);
            BlockchainAddress blockchainAddress = null;
            var dataReader = cmd.ExecuteReader();

            if (dataReader.Read())
            {
                blockchainAddress = new BlockchainAddress()
                {
                    Address = dataReader.GetString("address"),
                    PrivateKey = dataReader.GetString("privateKey"),
                    Balance = dataReader.GetDouble("balance")
                };

            }
            ConnectionHelper.CloseConnection();
            return blockchainAddress;
        }

        public bool UpdateBalanceBlockchain(BlockchainAddress currentLoggedInAddress,
            BlockchainTransaction blockchainTransaction)
        {
            ConnectionHelper.GetConnection();
            var transaction1 = ConnectionHelper.GetConnection().BeginTransaction();
            try
            {
                var cmd = new MySqlCommand("select balance from BlockChainAccount where address = @address",
                    ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                BlockchainAddress blockchainAddress = null;
                var dataReader = cmd.ExecuteReader();
                double currentAddressBalance = 0;
                if (dataReader.Read())
                {
                    currentAddressBalance = dataReader.GetDouble("balance");
                }

           
               
                dataReader.Close();
                if (currentAddressBalance < 0)
                {
                    Console.WriteLine("Không đủ tiền trong tài khoản.");
                    return false;
                }
                    
                if (blockchainTransaction.Type == BlockchainTransaction.TransactionType.WITHDRAW &&
                    currentAddressBalance < blockchainTransaction.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");

                }
                
                if (blockchainTransaction.Type == BlockchainTransaction.TransactionType.WITHDRAW)
                {
                    currentAddressBalance -= blockchainTransaction.Amount;
                }
                
                else if (blockchainTransaction.Type == BlockchainTransaction.TransactionType.DEPOSIT)
                {
                    currentAddressBalance += blockchainTransaction.Amount;
                }
                else if(blockchainTransaction.Type == BlockchainTransaction.TransactionType.TRANSFER)
                {
                   

                }
                
                var updateQuery =
                    "update `BlockChainAccount` set `balance` = @balance where address = @address";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAddressBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                var updateResult = sqlCmd.ExecuteNonQuery();
                
                var historyTransactionQuery =
                    "insert into `BlockChainTransaction` (transactionId, type, senderAddress, receiverAddress, amount, message) " +
                    "values (@transactionId, @type, @senderAddress, @receiverAddress, @amount, @message)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@transactionId", blockchainTransaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", blockchainTransaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", blockchainTransaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", blockchainTransaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@senderAddress",
                    blockchainTransaction.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAddress",
                    blockchainTransaction.ReceiverAddress);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateResult != 1 || historyResult != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                transaction1.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                transaction1.Rollback();                 
                return false;
            }
            ConnectionHelper.CloseConnection();
            return true;
        }
    }
    
}