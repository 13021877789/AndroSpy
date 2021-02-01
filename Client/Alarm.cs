using Android.Content;

namespace Task2
{
    [BroadcastReceiver(Exported = true)]
    class Alarm : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (MainActivity.mySocketConnected == false)
            {
                ((MainActivity)MainActivity.global_activity).Baglanti_Kur();
            }
        }
    }
}