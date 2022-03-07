using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace ZonBot.Modules.SlashCommands
{
    public class TestModuleData
    {
        #region Singleton
        private static TestModuleData s_instance;
        
        public static TestModuleData Get()
        {
            if (s_instance == null)
            {
                s_instance = new TestModuleData();
            }
            
            return s_instance;
        }
        #endregion

        public int Value = 0;
        
        private TestModuleData()
        {
        }
    }
    
    [Group("test", "TestModule")]
    public class TestModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
        [SlashCommand("inc", "inc value", runMode: RunMode.Async)]
        public async Task TemplateAsync()
        {
            await RespondAsync(TestModuleData.Get().Value++.ToString());
        }
    }
}