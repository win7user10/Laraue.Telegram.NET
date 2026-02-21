using System.Text;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;
using Xunit.Sdk;

namespace Laraue.Telegram.NET.Testing;

public static class AssertExtensions
{
    extension(EditMessageTextRequest request)
    {
        /// <summary>
        /// Asserts that client telegram message is equal to the passed. 
        /// </summary>
        public void CheckMessage(string expected)
        {
            Assert.Equal(expected, request.Text);
        }

        /// <summary>
        /// Asserts that client telegram message has exactly the passed rows count.
        /// </summary>
        public void HasButtonsRowsCount(int exceptedRowsCount)
        {
            Assert.Equal(exceptedRowsCount, GetButtons(request).Length);
        }

        /// <summary>
        /// Asserts the specific buttons of telegram message.
        /// </summary>
        /// <param name="buildAssert"></param>
        public void CheckButtons(Action<CheckButtonsAssert> buildAssert)
        {
            var checkButtonAssert = new CheckButtonsAssert();
            buildAssert(checkButtonAssert);

            var realButtons = GetButtons(request);
            foreach (var assert in checkButtonAssert.Asserts)
            {
                HasButtonRow(request, assert.Row, realButtons, assert.Asserts);
            }
        }
        
        /// <summary>
        /// Asserts all buttons rows of telegram message sequentially.
        /// Throw if any of rows have not been checked.
        /// </summary>
        public void CheckButtonsSequentially(Action<CheckSequentiallyButtonsAssert> buildAssert)
        {
            var checkButtonAssert = new CheckSequentiallyButtonsAssert();
            buildAssert(checkButtonAssert);

            var realButtons = GetButtons(request);
            for (var index = 0; index < checkButtonAssert.Asserts.Count; index++)
            {
                var assert = checkButtonAssert.Asserts[index];
                HasButtonRow(request, index, realButtons, assert);
            }

            var assertsCount = checkButtonAssert.Asserts.Count;
            if (realButtons.Length != assertsCount)
            {
                var sb = new StringBuilder();
                foreach (var nonCheckedRow in realButtons
                    .Skip(checkButtonAssert.Asserts.Count))
                {
                    sb
                        .Append('[')
                        .AppendJoin(", ", nonCheckedRow
                        .Select(nonCheckedButton =>
                            $"Text = {nonCheckedButton.Text}, CallbackData = {nonCheckedButton.CallbackData}"))
                        .Append(']');
                }

                var ex = CollectionException.ForMismatchedItemCount(
                    realButtons.Length,
                    assertsCount,
                    sb.ToString());

                throw new TelegramNetAssertException(
                    $"Not all items of collection checked. Investigate unchecked items below.{Environment.NewLine}{ex.Message}");
            }
        }

        private void HasButtonRow(
            int row,
            IEnumerable<InlineKeyboardButton>[] realButtons,
            params ButtonAssert[] asserts)
        {
            Assert.True(realButtons.Length >= row);
            
            var realRow = realButtons[row];
            var assertItems = realRow
                .Select(r => new ButtonAssert(
                    r.Text,
                    r.CallbackData));

            try
            {
                Assert.Equal(asserts, assertItems);
            }
            catch (Exception e)
            {
                throw new TelegramNetAssertException(
                    $"Assert for button row #{row + 1} failed.{Environment.NewLine}{e.Message}");
            }
        }

        private IEnumerable<InlineKeyboardButton>[] GetButtons()
        {
            return request
                .ReplyMarkup
                ?.InlineKeyboard.ToArray() ?? [];
        }
    }
}

/// <summary>
/// Assert data of a button.
/// </summary>
public record ButtonAssert(string Text, string? CallbackData);

/// <summary>
/// Assert data of a buttons row.
/// </summary>
public record ButtonRowAssert(int Row, params ButtonAssert[] Asserts);

public class CheckButtonsAssert
{
    internal readonly List<ButtonRowAssert> Asserts = new ();
    public CheckButtonsAssert HasButtonsRow(int row, params ButtonAssert[] asserts)
    {
        Asserts.Add(new(row, asserts));

        return this;
    }
}

public class CheckSequentiallyButtonsAssert
{
    internal readonly List<ButtonAssert[]> Asserts = new ();
    public CheckSequentiallyButtonsAssert HasButtonsRow(params ButtonAssert[] asserts)
    {
        Asserts.Add(asserts);

        return this;
    }
}