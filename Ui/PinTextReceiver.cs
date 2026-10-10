using System;

namespace NotThatBlindCartographer.Ui
{
    public class PinTextReceiver : TextReceiver
    {
        private readonly string _initial;
        private readonly Action<string> _onSubmit;

        public PinTextReceiver(string initial, Action<string> onSubmit)
        {
            _initial = initial;
            _onSubmit = onSubmit;
        }

        public string GetText()
        {
            return _initial;
        }

        public void SetText(string text)
        {
            _onSubmit?.Invoke(text);
        }
    }
}
