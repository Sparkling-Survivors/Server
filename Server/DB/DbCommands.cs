using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;
using ServerCore;

namespace Server.DB
{
    public class DbCommands
    {
        public static void InitializeDB()
        {
            using (AppDbContext db = new AppDbContext())
            {
                if ((db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                {
                    Console.WriteLine("db가 존재함!");
                }
            }
        }

        public static void CreateTestData()
        {
            using (AppDbContext db = new AppDbContext())
            {
                Setting setting = new Setting();
                setting.steamId = 1;
                setting.instanceId = 1;
                setting.mouseSensitivity = 1.0f;
                setting.isFullScreen = true;
                setting.displayQuality = 1;
                setting.width = 1920;
                setting.height = 1080;

                db.Setting.Add(setting);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 비동기로 Setting 테이블에서 인자로 받은 steamId에 해당하는 데이터를 찾아서 패킷으로 클라한테 보내줌
        /// </summary>
        /// <param name="session"></param>
        /// <param name="steamId"></param>
        /// <returns>테이블에 없는 steamId라면 isSettingExist가 false로 담겨서 감</returns>
        public static async Task<Setting> GetSetting(ClientSession session , ulong steamId)
        {
            using (AppDbContext db = new AppDbContext())
            {
                Setting settingInfo = await db.Setting.FirstOrDefaultAsync(s => s.steamId == steamId);
                if (settingInfo == null)
                {
                    // steamId에 해당하는 데이터가 없는 경우
                    Console.WriteLine($"No setting found for the given steamId {steamId}.");

                    //setting정보 없다고 패킷 보내기
                    SC_GetSetting sendPacket = new SC_GetSetting();
                    sendPacket.IsSettingExist = false;

                    session.Send(sendPacket);
                }
                else
                {
                    SC_GetSetting sendPacket = new SC_GetSetting();
                    sendPacket.IsSettingExist = true;
                    sendPacket.SteamId = settingInfo.steamId;
                    sendPacket.InstanceId = settingInfo.instanceId;
                    sendPacket.MouseSensitivity = settingInfo.mouseSensitivity;
                    sendPacket.IsFullScreen = settingInfo.isFullScreen;
                    sendPacket.DisplayQuality = settingInfo.displayQuality;
                    sendPacket.Width = settingInfo.width;
                    sendPacket.Height = settingInfo.height;

                    session.Send(sendPacket);
                }

                return settingInfo;
            }
        }

        //비동기로 Setting테이블에 클라한테 packet으로 받은 정보를 저장함
        public static async Task SetSetting(ClientSession session, CS_SetSetting packet)
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // 해당 steamId가 있는지 확인
                    var existingSetting = await db.Setting.FirstOrDefaultAsync(s => s.steamId == packet.SteamId);

                    if (existingSetting == null)
                    {
                        // 없으면 새로 추가
                        Setting newSetting = new Setting
                        {
                            steamId = packet.SteamId,
                            instanceId = packet.InstanceId,
                            mouseSensitivity = packet.MouseSensitivity,
                            isFullScreen = packet.IsFullScreen,
                            displayQuality = packet.DisplayQuality,
                            width = packet.Width,
                            height = packet.Height
                        };
                        Console.WriteLine($"Add {packet.SteamId}'s setting");
                        await db.Setting.AddAsync(newSetting);
                    }
                    else
                    {
                        Console.WriteLine($"Update {packet.SteamId}'s setting ");
                        // 있으면 기존 값을 업데이트
                        existingSetting.instanceId = packet.InstanceId;
                        existingSetting.mouseSensitivity = packet.MouseSensitivity;
                        existingSetting.isFullScreen = packet.IsFullScreen;
                        existingSetting.displayQuality = packet.DisplayQuality;
                        existingSetting.width = packet.Width;
                        existingSetting.height = packet.Height;

                        db.Setting.Update(existingSetting);
                    }

                    // 변경 사항을 저장
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred: " + ex.Message);
            }
        }

    }

}
