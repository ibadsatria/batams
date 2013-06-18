using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DbConn
{
    // Class Name   : Program
    // Creator      : Iwan Supratman (deong)
    // Create Date  : June, 13 2013
    // Mail         : deong84@gmail.com
    // Globat Matra Aditama, PT.

    class Program
    {
        static DbCommon db;
        static AppQuery qr;
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    // 1. koneksi ke database
                    Console.WriteLine("1. Connecting database ...");
                    db = new DbCommon("localhost", "attendance_batam", "postgres", "huntu");
                    if (db.ConnectDb() == 1)
                    {
                        Console.WriteLine("Database connected.");
                    }
                    else
                    {
                        Console.WriteLine("Database not connected.");
                        break;
                    }
                    
                    // 2. initial query constructor
                    Console.WriteLine("2. Initial query constructor.");
                    qr = new AppQuery(db);

                    Console.WriteLine("\r\n");

                    // 3. test insert database
                    Console.WriteLine("3. Test insert database : ");
                    Console.WriteLine("------------------------------------------------------------");
                    if (qr.TestInsertTable() == 0)
                    {
                        Console.WriteLine("Gagal insert!.");
                    }
                    else
                    {
                        Console.WriteLine("Berhasil insert!.");
                    }
                    
                    Console.WriteLine("\r\n");

                    // 4. test update database
                    Console.WriteLine("4. Test update database : ");
                    Console.WriteLine("------------------------------------------------------------");
                    if (qr.TestUpdateTable() == 0)
                    {
                        Console.WriteLine("Gagal update!.");
                    }
                    else
                    {
                        Console.WriteLine("Berhasil update!.");
                    }

                    Console.WriteLine("\r\n");

                    // 5. test baca database pada table beberapa row (limit offset) dengan kondisi WHERE
                    // di gunakan untuk eksekusi query dengan banyak return data row.
                    Console.WriteLine("5. Test baca database pada table beberapa row : ");
                    Console.WriteLine("------------------------------------------------------------");
                    DataTable record = qr.TestReadTable();
                    if (record == null)
                    {
                        Console.WriteLine("Gagal baca!.");
                    }
                    else
                    {
                        Console.WriteLine("Berhasil baca!.");
                        // coba keluarin data nya ke consol
                        Console.WriteLine("-------------------------------------------------");
                        for (int i = 0; i < record.Rows.Count; i++)
                        {
                            DataRow row = record.Rows[i];
                            int j = 0;
                            Console.Write("|");
                            foreach (object item in row.ItemArray)
                            {
                                if (item is int)
                                    Console.Write(item.ToString() + "    |    ");
                                else if (item is string)
                                    Console.Write(item + "    |    ");
                                j++;
                            }
                            Console.WriteLine("\r\n-------------------------------------------------");
                        }
                    }

                    Console.WriteLine("\r\n");

                    // 6. test get one data only
                    // di gunakan untuk eksekusi query dengan return 1 data row saja.
                    Console.WriteLine("6. Test get one data only : ");
                    Console.WriteLine("------------------------------------------------------------");
                    string data = qr.TestDbGetOne();
                    if (data == "")
                    {
                        Console.WriteLine("Gagal baca!.");
                    }
                    else
                    {
                        Console.WriteLine("One Data = {0}", data);
                    }

                    Console.WriteLine("\r\n");

                    // 7. Test Free Execute Query
                    // di gunakan untuk eksekusi query tanpa return data row misal insert, update & delete data.
                    Console.WriteLine("7. Test Free Execute Query : ");
                    Console.WriteLine("------------------------------------------------------------");
                    string mysql = "UPDATE attendance_course SET course_id = 'EL-3407' WHERE id = 'A15A04'";
                    int res = qr.DbFreeExecute("attendance_course", mysql);
                    if (res == 0)
                    {
                        Console.WriteLine("Eksekusi query gagal!.");
                    }
                    else
                    {
                        Console.WriteLine("Eksekusi query berhasil!.");
                    }

                    Console.WriteLine("\r\n");
                    // 8. Test Free Query
                    // di gunakan untuk eksekusi query dengan return banyak data row.
                    Console.WriteLine("8. Test Free Query : ");
                    Console.WriteLine("------------------------------------------------------------");
                    // misalnya cari data yang statusnya masih 0
                    mysql = "SELECT * FROM attendance_course a WHERE a.status = 0 ";
                    record = qr.DbFreeQuery("attendance_course", mysql);
                    if (record == null)
                    {
                        Console.WriteLine("Gagal baca!.");
                    }
                    else
                    {
                        Console.WriteLine("Berhasil baca!.");
                        // coba keluarin data nya ke consol
                        Console.WriteLine("-------------------------------------------------");
                        for (int i = 0; i < record.Rows.Count; i++)
                        {
                            DataRow row = record.Rows[i];
                            int j = 0;
                            Console.Write("|");
                            foreach (object item in row.ItemArray)
                            {
                                if (item is int)
                                    Console.Write(item.ToString() + "    |    ");
                                else if (item is string)
                                    Console.Write(item + "    |    ");
                                j++;
                            }
                            Console.WriteLine("\r\n-------------------------------------------------");
                        }
                    }

                    Console.WriteLine("\r\n");

                    // 9. test delete data
                    Console.WriteLine("9. Test delete data yang tadi di insert : ");
                    Console.WriteLine("------------------------------------------------------------");
                    if (qr.TestDeleteRow() == 0)
                    {
                        Console.WriteLine("Gagal update!.");
                    }
                    else
                    {
                        Console.WriteLine("Berhasil update!.");
                    }

                    Console.WriteLine("\r\n");

                    // 10. disconnect database
                    Console.WriteLine("10. Disconnect from database. ");
                    Console.WriteLine("------------------------------------------------------------");
                    if (db.DisconnectDb() == 0)
                    {
                        Console.WriteLine("Gagal disconnect!.");
                    }
                    else
                    {
                        Console.WriteLine("Berhasil disconnect!.");
                    }

                    Console.Write("All proccess is done, you want try again [y/n] ? ");
                    string key = Console.ReadLine();
                    if (key.ToLower() == "y")
                        Console.Clear();
                    else
                        break;
                                                              
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message.ToString());
                }
            }            
        }
    }
}
