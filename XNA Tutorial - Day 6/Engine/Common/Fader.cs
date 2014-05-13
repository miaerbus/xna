using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Artificial.XNATutorial
{
    public class Fader
    {
        float min;
        float max;
        float speed;
        float direction;

        float value;
        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        bool stopped = true;
        public bool Finished
        {
            get
            {
                return stopped;
            }
        }

        public Fader(float min, float max, float fadeDuration)
        {
            this.min = min;
            this.max = max;
            value = min;
            speed = (max - min) / fadeDuration;
        }

        public void Update(float elapsedSeconds)
        {
            if (stopped) return;

            value += speed * direction * elapsedSeconds;
            if (direction > 0)
            {
                if (Math.Sign(speed) * value > Math.Sign(speed) * max)
                {
                    value = max;
                    stopped = true;
                }
            }
            else
            {
                if (Math.Sign(speed) * value < Math.Sign(speed) * min)
                {
                    value = min;
                    stopped = true;
                }
            }
        }

        public void FadeToMax()
        {
            stopped = false;
            direction = 1;
        }

        public void FadeToMin()
        {
            stopped = false;
            direction = -1;
        }
    }
}
