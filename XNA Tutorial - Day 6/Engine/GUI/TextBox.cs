using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;
using FontRenderer;

namespace Artificial.XNATutorial.GUI
{
    public class TextBox : TextButton
    {
        bool EditMode = false;
        int cursor = 0;
        string editableText;

        GUIAction onEditFinished;

        public TextBox(Vector3 position, string text, float scale, TextPosition textPosition, Font font, GUIAction onEditFinished, GUIAction onHover)
        {
            this.onEditFinished = onEditFinished;
            base.Create(position, text, scale, textPosition, font, StartEdit, onHover);
        }

        protected override void update(float dt)
        {
            base.update(dt);
            if (EditMode)
            {
                mouseHover = true;
                if (GlobalInput.WasKeyPressed(Keys.Left)) CursorLeft();
                if (GlobalInput.WasKeyPressed(Keys.Right)) CursorRight();
                if (GlobalInput.WasKeyPressed(Keys.Delete)) Delete();
                if (GlobalInput.WasKeyPressed(Keys.Back)) Backspace();
                if (GlobalInput.WasKeyPressed(Keys.Enter)) EndEdit();
                if (GlobalInput.WasKeyPressed(Keys.A)) AddChar('A');
                if (GlobalInput.WasKeyPressed(Keys.B)) AddChar('B');
                if (GlobalInput.WasKeyPressed(Keys.C)) AddChar('C');
                if (GlobalInput.WasKeyPressed(Keys.D)) AddChar('D');
                if (GlobalInput.WasKeyPressed(Keys.E)) AddChar('E');
                if (GlobalInput.WasKeyPressed(Keys.F)) AddChar('F');
                if (GlobalInput.WasKeyPressed(Keys.G)) AddChar('G');
                if (GlobalInput.WasKeyPressed(Keys.H)) AddChar('H');
                if (GlobalInput.WasKeyPressed(Keys.I)) AddChar('I');
                if (GlobalInput.WasKeyPressed(Keys.J)) AddChar('J');
                if (GlobalInput.WasKeyPressed(Keys.K)) AddChar('K');
                if (GlobalInput.WasKeyPressed(Keys.L)) AddChar('L');
                if (GlobalInput.WasKeyPressed(Keys.M)) AddChar('M');
                if (GlobalInput.WasKeyPressed(Keys.N)) AddChar('N');
                if (GlobalInput.WasKeyPressed(Keys.O)) AddChar('O');
                if (GlobalInput.WasKeyPressed(Keys.P)) AddChar('P');
                if (GlobalInput.WasKeyPressed(Keys.Q)) AddChar('Q');
                if (GlobalInput.WasKeyPressed(Keys.R)) AddChar('R');
                if (GlobalInput.WasKeyPressed(Keys.S)) AddChar('S');
                if (GlobalInput.WasKeyPressed(Keys.T)) AddChar('T');
                if (GlobalInput.WasKeyPressed(Keys.U)) AddChar('U');
                if (GlobalInput.WasKeyPressed(Keys.V)) AddChar('V');
                if (GlobalInput.WasKeyPressed(Keys.W)) AddChar('W');
                if (GlobalInput.WasKeyPressed(Keys.X)) AddChar('X');
                if (GlobalInput.WasKeyPressed(Keys.Y)) AddChar('Y');
                if (GlobalInput.WasKeyPressed(Keys.Z)) AddChar('Z');
                if (GlobalInput.WasKeyPressed(Keys.Space)) AddChar(' ');
            }
        }

        void StartEdit(Item sender)
        {
            if (!EditMode)
            {
                EditMode = true;
                cursor = Text.Length;
                editableText = text;
                RefreshText();
            }
        }

        void EndEdit()
        {
            EditMode = false;
            Text = editableText;
            if (onEditFinished != null) onEditFinished(this);
        }

        void AddChar(char c)
        {
            editableText = editableText.Substring(0, cursor) + c + editableText.Substring(cursor);
            cursor++;
            RefreshText();
        }

        void CursorLeft()
        {
            cursor--;
            if (cursor < 0) cursor = 0;
            RefreshText();
        }

        void CursorRight()
        {
            cursor++;
            if (cursor > editableText.Length) cursor = editableText.Length;
            RefreshText();
        }

        void Delete()
        {
            if (cursor < editableText.Length)
            {
                editableText = editableText.Substring(0, cursor) + editableText.Substring(cursor + 1);
                RefreshText();
            }
        }

        void Backspace()
        {
            if (cursor > 0)
            {
                cursor--;
                Delete();
            }
        }


        void RefreshText()
        {
            Text = editableText.Substring(0, cursor) + "|" + editableText.Substring(cursor);
        }
    }
}
