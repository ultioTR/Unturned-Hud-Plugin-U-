using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Rocket.Core.Logging;

namespace TestPlugin
{
    // 1. Ana Plugin Sınıfı
    public class TestPlugin : RocketPlugin<TestPluginConfiguration>
    {
        // Plugin yüklendiğinde çalışır
        protected override void Load()
        {
            // Sunucu konsoluna bir mesaj yazdır
            Logger.Log("#############################################", System.ConsoleColor.Yellow);
            Logger.Log($"{Name} {Assembly.GetName().Version} başarıyla yüklendi!", System.ConsoleColor.Green);
            Logger.Log($"Yapılandırma mesajı: {Configuration.Instance.TestMesaji}", System.ConsoleColor.Cyan);
            Logger.Log("#############################################", System.ConsoleColor.Yellow);
        }

        // Plugin kaldırıldığında/sunucu kapandığında çalışır
        protected override void Unload()
        {
            Logger.Log($"{Name} başarıyla kaldırıldı!", System.ConsoleColor.Red);
        }

        // Plugin için varsayılan çevirileri tanımlar. 
        // Bu, çok dilli destek veya komut mesajlarını kolayca yönetmek için kullanılır.
        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "merhaba_mesaji", "Merhaba {0}! Test komutu başarıyla çalıştı." },
            { "konsol_kullanamaz", "Bu komutu sadece oyun içinden kullanabilirsiniz." }
        };
    }

    // 2. Yapılandırma Sınıfı
    // Plugins/TestPlugin/TestPlugin.configuration.xml dosyasını yönetir.
    public class TestPluginConfiguration : IRocketPluginConfiguration
    {
        // Yapılandırma dosyasına eklenecek bir ayar
        public string TestMesaji { get; set; }

        // Varsayılan ayarları yükler
        public void LoadDefaults()
        {
            TestMesaji = "Bu bir test mesajıdır!";
        }
    }

    // 3. Komut Sınıfı
    // Oyuncuların kullanabileceği komutları tanımlar.
    public class CommandTest : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player; // Komutu kimlerin kullanabileceği (Player, Console, Both)
        public string Name => "test"; // Komutun adı (/test)
        public string Help => "Basit bir test komutu."; // Komutun yardım metni (/help test)
        public string Syntax => "/test"; // Komutun doğru kullanım şekli
        public List<string> Aliases => new List<string>() { "testet" }; // Komutun diğer adları (/testet)
        public List<string> Permissions => new List<string>() { "testplugin.test" }; // Komutu kullanmak için gereken izin

        public void Execute(IRocketPlayer caller, string[] command)
        {
            // Bu komut sadece oyuncular tarafından kullanılabilir.
            if (caller is not UnturnedPlayer player)
            {
                // Eğer konsoldan çalıştırılırsa çeviri listesindeki mesajı gönder.
                UnturnedChat.Say(caller, TestPlugin.Instance.Translate("konsol_kullanamaz"));
                return;
            }

            // Çeviri listesinden "merhaba_mesaji"nı al, {0} yerine oyuncunun adını koy ve ekrana yazdır.
            string message = TestPlugin.Instance.Translate("merhaba_mesaji", player.CharacterName);
            UnturnedChat.Say(player, message, UnityEngine.Color.green);
        }
    }
}
