namespace Deep.Controls.Exceptions;

public class ElementNotFoundOnStyleException(string elementName) : Exception($"\"{elementName}\" not found on Style")
{
}