using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//
// This is where you put your code.  There are two sections, one for members to add to the Serializer class,
// and one for members to add to the Deserializer class.
//
namespace Assets.Serialization
{
    // The partial keyword just means we're adding these three methods to the code in Serializer.cs
    public partial class Serializer
    {
        /// <summary>
        /// Print out the serialization data for the specified object.
        /// </summary>
        /// <param name="o">Object to serialize</param>
        private void WriteObject(object o)
        {
            switch (o)
            {
                case null:
                    // My code
                    //?
                    Write("null");
                    break;

                case int i:
                    Write(i);
                    break;

                case float f:
                    Write(f);
                    break;

                // BUG: this doesn't handle strings that themselves contain quote marks
                // but that doesn't really matter for an assignment like this, so I'm not
                // going to confuse the reader by complicating the code to escape the strings.
                case string s:
                    Write("\"" + s + "\"");
                    break;

                case bool b:
                    Write(b);
                    break;

                case IList list:
                    WriteList(list);
                    break;

                default:
                    if (o.GetType().IsValueType)
                        throw new Exception($"Trying to write an unsupported value type: {o.GetType().Name}");

                    WriteComplexObject(o);
                    break;
            }
        }

        /// <summary>
        /// Serialize a complex object (i.e. a class object)
        /// If this object has already been output, just output #id, where is is it's id from GetID.
        /// If it hasn't then output #id { type: "typename", field: value ... }
        /// </summary>
        /// <param name="o">Object to serialize</param>
        ///

        //#0{
        //        type: "FakeGameObject",
        //    name: "test",
        //    components: [
        //#1{
        //            type: "FakeTransform",
        //            X: 0,
        //            Y: 0,
        //            parent: null,
        //            children: [
        //#2{
        //                    type: "FakeTransform",
        //                    X: 100,
        //                    Y: 100,
        //                    parent: #1,
        //                    children: [
        //#3{
        //                            type: "FakeTransform",
        //                            X: 0,
        //                            Y: 0,
        //                            parent: #2,
        Dictionary<object, int> dict = new Dictionary<object, int>();
        int num = 0;

        private void WriteComplexObject(object o)
        {
            // This is the method that gets called to serialize objects that
            // have fields in them.
            // -You will need to assign the object a serial
            // number, if you haven’t already, and
            // remember that serial number in a hash table (use the Dictionary<object,int> data type;
            // search for “C# Dictionary class” for documentation).
            // -If there’s already a serial number assigned to the object, then you’ve
            // already written the object once in this serialization, so just
            // write out a # followed by the number.
            // -If you haven’t already assigned a number, assign one, remember it, write out # followed
            // by the number, and the write { }s with the type and fields
            // written inside, separated by commas.


            //string a = o.GetType().Name;

            //SerializedFields(o);
            IEnumerable<KeyValuePair<string, object>> fields = Utilities.SerializedFields(o);
            //UnityEngine.Debug.Log("hi ");

            //Dictionary<object, int> dict = new Dictionary<object, int>();
            //dict.Add(o, 0);
            //indentLevel = 0;
            
            dict.Add(o, num);
            num++;


            foreach (var field in fields)
            {
                WriteField(field.Key, field.Value, false);
                try
                {
                    UnityEngine.Debug.Log("Fields - Key: " + field.Key + " Value: " + field.Value);
                    //UnityEngine.Debug.Log("Fields - Object Name: " + field.Value.GetType().Name);

                    //if (dict.ContainsKey(field.Value))
                    //{
                    //    dict.Add("null", num);

                    //}
                    //else
                    //{
                    //    dict.Add(field.Value, num);
                    //    WriteField(field.Key, field.Value, false);
                        //Add the object to the dictionary with assigned serial number if it doesn't already exist
                        // implement TryAdd(), fix handling null value
                        //WriteField(string fieldName, object fieldValue, bool firstOne)
                        //try
                        //{
                        //    WriteField(field.Key, field.Value, false);
                        //    //if ((!dict.ContainsKey(field.Value)))
                        //    //{

                        //    dict.Add(field.Value, num);
                        //    //} 
                        //    //UnityEngine.Debug.Log("[fields] Key: " + field.Key + " Value: " + field.Value);
                        //}
                        //catch (ArgumentNullException)
                        //{
                        //    UnityEngine.Debug.Log("I got nothing");
                        //    dict.Add("null", num);
                        //}
                        
                    //}
                }
                catch (ArgumentNullException)
                {
                    UnityEngine.Debug.Log("I got nothing");
                    //dict.Add("null", num);

                }
                //num++;
            }
            //print dict
            //UnityEngine.Debug.Log(dict.ToString);
            foreach (KeyValuePair<object, int> page in dict)
            {
                UnityEngine.Debug.Log("[dict] Key: " + page.Key + "\nValue: " + page.Value);

            }

            // Step: Check if object has already been serialized
            // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-5.0 
            // Dictionary.ContainsKey(TKey)
            // Dictionary.ContainsValue(TValue)
            // Dictionary.Equals()


            // Step: Assign a serial number to the object
            // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=net-5.0
            //List<int> serialNums = new List<int>();
            //last list value +1


            // Step: Put object and serial number into Dictionary


            // Step: Print #Serial num{ \n "type: ", type, "name: ", name, "components: ["
            // Serializer.indentLevel, Serializer.NewLine(), Serializer.WriteField(),
            // Serializer.WriteBracketedExpression(), Serializer.WriteList()
        }
    }

    // The partial keyword just means we're adding these three methods to the code in Deserializer.cs
    public partial class Deserializer
    {
        /// <summary>
        /// Read whatever data object is next in the stream
        /// </summary>
        /// <param name="enclosingId">The object id of whatever object this is a part of, if any</param>
        /// <returns>The deserialized object</returns>
        public object ReadObject(int enclosingId)
        {
            SkipWhitespace();

            if (End)
                throw new EndOfStreamException();

            switch (PeekChar)
            {
                case '#':
                    return ReadComplexObject(enclosingId);

                case '[':
                    return ReadList(enclosingId);

                case '"':
                    return ReadString(enclosingId);

                case '-':
                case '.':
                case var c when char.IsDigit(c):
                    return ReadNumber(enclosingId);

                case var c when char.IsLetter(c):
                    return ReadSpecialName(enclosingId);

                default:
                    throw new Exception($"Unexpected character {PeekChar} found while reading object id {enclosingId}");
            }
        }

        /// <summary>
        /// Called when the next character is a #.  Read the object id of the object and return the
        /// object.  If that object id has already been read, return the object previously returned.
        /// Otherwise, there will be a { } expression after the object id.  Read it, create the object
        /// it represents, and return it.
        /// </summary>
        /// <param name="enclosingId">Object id of the object this expression appears inside of, if any.</param>
        /// <returns>The object referred to by this #id expression.</returns>
        private object ReadComplexObject(int enclosingId)
        {
            GetChar();  // Swallow the #
            var id = (int)ReadNumber(enclosingId);
            SkipWhitespace();

            // You've got the id # of the object.  Are we done now?
            throw new NotImplementedException("Fill me in");

            // Assuming we aren't done, let's check to make sure there's a { next
            SkipWhitespace();
            if (End)
                throw new EndOfStreamException($"Stream ended after reference to unknown ID {id}");
            var c = GetChar();
            if (c != '{')
                throw new Exception($"Expected '{'{'}' after #{id} but instead got {c}");

            // There's a {.
            // Let's hope there's a type: typename line.
            var (hopefullyType, typeName) = ReadField(id);
            if (hopefullyType != "type")
                throw new Exception(
                    $"Expected type name at the beginning of complex object id {id} but instead got {typeName}");
            var type = typeName as string;
            if (type == null)
                throw new Exception(
                    $"Expected a type name (a string) in 'type: ...' expression for object id {id}, but instead got {typeName}");

            // Great!  Now what?
            throw new NotImplementedException("Fill me in");

            // Read the fields until we run out of them
            while (!End && PeekChar != '}')
            {
                var (field, value) = ReadField(id);
                // We've got a field and a value.  Now what?
                throw new NotImplementedException("Fill me in");
            }

            if (End)
                throw new EndOfStreamException($"Stream ended in the middle of {"{ }"} expression for id #{id}");

            GetChar();  // Swallow close bracket

            // We're done.  Now what?
            throw new NotImplementedException("Fill me in");
        }
    }
}
