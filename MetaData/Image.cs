// *******************************************************************************
// 
//  *******   ***   ***               *
//     *     *     *                  *
//     *    *      *                *****
//     *    *       ***  *   *   **   *    **    ***
//     *    *          *  * *   *     *   ****  * * *
//     *     *         *   *      *   * * *     * * *
//     *      ***   ***    *     **   **   **   *   *
//                         *
// *******************************************************************************
//  see https://github.com/tgoessler/TCSystemCS for details.
//  Copyright (C) 2003 - 2026 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCSystem.Util;

#endregion

namespace TCSystem.MetaData;

/// <summary>Represents the immutable metadata associated with an image file.</summary>
/// <param name="_fileId">The persistent file identifier.</param>
/// <param name="_fileName">The full file name.</param>
/// <param name="_processingInfos">The completed processing operations.</param>
/// <param name="_width">The image width in pixels.</param>
/// <param name="_height">The image height in pixels.</param>
/// <param name="_orientation">The stored image orientation.</param>
/// <param name="_dateTaken">The date and time the image was taken.</param>
/// <param name="_title">The image title.</param>
/// <param name="_location">The optional image location.</param>
/// <param name="_personsTags">The associated person tags.</param>
/// <param name="_tags">The associated text tags.</param>
public sealed class Image(long _fileId, string _fileName, ProcessingInfos _processingInfos, int _width,
                          int _height, OrientationMode _orientation, DateTimeOffset _dateTaken, string _title,
                          Location _location, IReadOnlyList<PersonTag> _personsTags, IReadOnlyList<string> _tags) : IEquatable<Image>
{
#region Public

    /// <summary>Determines whether the image has a person with the specified name.</summary>
    public bool HasPerson(string name)
    {
        return GetPersonTag(name) != null;
    }

    /// <summary>Determines whether the image contains an equal person tag.</summary>
    public bool HasPersonTag(PersonTag personTag)
    {
        return _personTags.FirstOrDefault(p => p.Equals(personTag)) != null;
    }

    /// <summary>Gets the first person tag with the specified name.</summary>
    /// <returns>The matching tag, or <see langword="null" />.</returns>
    public PersonTag GetPersonTag(string name)
    {
        return _personTags.FirstOrDefault(pt => pt.Person.Name == name);
    }

    /// <summary>Determines whether the image contains the specified text tag.</summary>
    public bool HasTag(string t)
    {
        return _tags.FirstOrDefault(x => x == t) != null;
    }

    /// <summary>Returns a copy with a different file name.</summary>
    public static Image ChangeFileName(Image image, string fileName)
    {
        return new(image.Id, fileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            image._personTags, image._tags);
    }

    /// <summary>Returns a copy with a different date taken.</summary>
    public static Image ChangeDateTaken(Image image, DateTimeOffset dateTaken)
    {
        return new(image.Id, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            dateTaken, image.Title, image.Location,
            image._personTags, image._tags);
    }

    /// <summary>Returns a copy with different processing information.</summary>
    public static Image ChangeProcessingInfo(Image image, ProcessingInfos processingInfos)
    {
        return new(image.Id, image.FileName, processingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            image._personTags, image._tags);
    }

    /// <summary>Returns a copy with a different location.</summary>
    public static Image ChangeLocation(Image image, Location location)
    {
        return new(image.Id, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, location,
            image._personTags, image._tags);
    }

    /// <summary>Returns a copy with the supplied person tags.</summary>
    public static Image ChangePersonTags(Image image, IEnumerable<PersonTag> personTags)
    {
        return new(image.Id, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            personTags.ToArray(), image._tags);
    }

    /// <summary>Returns a copy with the supplied text tags.</summary>
    public static Image ChangeTags(Image image, IEnumerable<string> tagsIn)
    {
        return new(image.Id, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            image._personTags, tagsIn.ToArray());
    }

    /// <summary>Returns a copy containing the person tag, unless a valid person with the same name already exists.</summary>
    public static Image AddPersonTag(Image image, PersonTag pt)
    {
        if (!pt.Person.IsValid || image._personTags.FirstOrDefault(x => x.Person.Name == pt.Person.Name) == null)
        {
            List<PersonTag> pts = image._personTags.ToList();
            pts.Add(pt);

            image = new(image.Id, image.FileName, image.ProcessingInfos,
                image.Width, image.Height, image.Orientation,
                image.DateTaken, image.Title, image.Location,
                pts, image._tags);
        }

        return image;
    }

    /// <summary>Returns a copy without the person tag, or the original image when it is absent.</summary>
    public static Image RemovePersonTag(Image image, PersonTag pt)
    {
        if (image.HasPersonTag(pt))
        {
            List<PersonTag> pts = image._personTags.ToList();
            pts.Remove(pt);

            image = new(image.Id, image.FileName, image.ProcessingInfos,
                image.Width, image.Height, image.Orientation,
                image.DateTaken, image.Title, image.Location,
                pts, image._tags);
        }

        return image;
    }

    /// <summary>Returns a copy with a person tag's face visibility changed.</summary>
    public static Image ChangePersonTagVisible(Image image, PersonTag pt, bool visible)
    {
        if (image.HasPersonTag(pt))
        {
            List<PersonTag> pts = image._personTags.ToList();
            pts.Remove(pt);
            pt = new(pt.Person, new(pt.Face.Id, pt.Face.Rectangle, pt.Face.FaceMode, pt.Face.FaceQuality, visible, pt.Face.FaceDescriptor));
            pts.Add(pt);

            image = new(image.Id, image.FileName, image.ProcessingInfos,
                image.Width, image.Height, image.Orientation,
                image.DateTaken, image.Title, image.Location,
                pts, image._tags);
        }

        return image;
    }

    /// <summary>Returns a copy with a person tag's face quality changed.</summary>
    public static Image ChangePersonTagFaceQuality(Image image, PersonTag pt, FaceQuality quality)
    {
        if (image.HasPersonTag(pt))
        {
            List<PersonTag> pts = image._personTags.ToList();
            pts.Remove(pt);
            pt = new(pt.Person, new(pt.Face.Id, pt.Face.Rectangle, pt.Face.FaceMode, quality, pt.Face.Visible, pt.Face.FaceDescriptor));
            pts.Add(pt);

            image = new(image.Id, image.FileName, image.ProcessingInfos,
                image.Width, image.Height, image.Orientation,
                image.DateTaken, image.Title, image.Location,
                pts, image._tags);
        }

        return image;
    }

    /// <summary>Returns a copy in which the person with the same name is replaced.</summary>
    public static Image ChangePerson(Image image, Person person)
    {
        if (image.GetPersonTag(person.Name) is { } pt)
        {
            List<PersonTag> pts = image._personTags.ToList();
            pts.Remove(pt);
            pt = new(person, pt.Face);
            pts.Add(pt);

            image = new(image.Id, image.FileName, image.ProcessingInfos,
                image.Width, image.Height, image.Orientation,
                image.DateTaken, image.Title, image.Location,
                pts, image._tags);
        }

        return image;
    }

    /// <summary>Returns a copy without the person having the specified name.</summary>
    public static Image RemovePersonWithName(Image image, string name)
    {
        PersonTag person = image.GetPersonTag(name);
        if (person != null)
        {
            List<PersonTag> pts = image._personTags.ToList();
            pts.Remove(person);

            image = new(image.Id, image.FileName, image.ProcessingInfos,
                image.Width, image.Height, image.Orientation,
                image.DateTaken, image.Title, image.Location,
                pts.AsReadOnly(), image._tags);
        }

        return image;
    }

    /// <summary>Returns a copy containing the non-empty text tag, or the original image if it already exists.</summary>
    public static Image AddTag(Image image, string t)
    {
        if (t.Length > 0 && !image.HasTag(t))
        {
            List<string> tags = image._tags.ToList();
            tags.Add(t);

            image = new(image.Id, image.FileName, image.ProcessingInfos,
                image.Width, image.Height, image.Orientation,
                image.DateTaken, image.Title, image.Location,
                image._personTags, tags.AsReadOnly());
        }

        return image;
    }

    /// <summary>Returns a copy without the text tag, or the original image when it is absent.</summary>
    public static Image RemoveTag(Image image, string t)
    {
        if (image.HasTag(t))
        {
            List<string> tags = image._tags.ToList();
            tags.Remove(t);

            image = new(image.Id, image.FileName, image.ProcessingInfos,
                image.Width, image.Height, image.Orientation,
                image.DateTaken, image.Title, image.Location,
                image._personTags, tags.AsReadOnly());
        }

        return image;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as Image, EqualsImp);
    }

    /// <inheritdoc />
    public bool Equals(Image other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = _personTags != null ? _personTags.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (_tags != null ? _tags.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Id.GetHashCode();
            hashCode = (hashCode * 397) ^ FileName.GetHashCode();
            hashCode = (hashCode * 397) ^ ProcessingInfos.GetHashCode();
            hashCode = (hashCode * 397) ^ Width.GetHashCode();
            hashCode = (hashCode * 397) ^ Height.GetHashCode();
            hashCode = (hashCode * 397) ^ Orientation.GetHashCode();
            hashCode = (hashCode * 397) ^ DateTaken.GetHashCode();
            hashCode = (hashCode * 397) ^ Title.GetHashCode();
            hashCode = (hashCode * 397) ^ (Location != null ? Location.GetHashCode() : 0);
            return hashCode;
        }
    }

    /// <summary>Serializes the image metadata to indented JSON.</summary>
    /// <returns>The formatted JSON.</returns>
    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    /// <summary>Serializes the image metadata to compact JSON.</summary>
    /// <returns>The serialized metadata.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Deserializes image metadata from JSON.</summary>
    /// <returns>The image metadata, or <see langword="null" /> for null or empty input.</returns>
    public static Image FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Gets the persistent file identifier.</summary>
    public long Id => _fileId;

    /// <summary>Gets the final component of the Windows-style file name.</summary>
    public string Name => FileName.Substring(FileName.LastIndexOf('\\') + 1);

    /// <summary>Gets the full file name.</summary>
    public string FileName => _fileName;

    /// <summary>Gets the completed processing operations.</summary>
    public ProcessingInfos ProcessingInfos => _processingInfos;

    /// <summary>Gets the image width in pixels.</summary>
    public int Width => _width;

    /// <summary>Gets the image height in pixels.</summary>
    public int Height => _height;

    /// <summary>Gets the stored image orientation.</summary>
    public OrientationMode Orientation => _orientation;

    /// <summary>Gets the date taken, trimmed to whole seconds.</summary>
    public DateTimeOffset DateTaken => _dateTaken.Trim(TimeSpan.TicksPerSecond);

    /// <summary>Gets whether a date taken has been set.</summary>
    public bool IsDateTimeSet => DateTaken != InvalidDateTaken;

    /// <summary>Gets the sentinel value representing an unset date taken.</summary>
    public static DateTimeOffset InvalidDateTaken => DateTimeOffset.FromUnixTimeSeconds(0).ToLocalTime();

    /// <summary>Gets the image title, or an empty string when undefined.</summary>
    public string Title => _title ?? "";

    /// <summary>Gets the optional image location.</summary>
    public Location Location => _location;

    /// <summary>Gets the associated person tags.</summary>
    public IReadOnlyList<PersonTag> PersonTags => _personTags;

    /// <summary>Gets the number of text tags.</summary>
    public int NumTags => _tags.Count;

    /// <summary>Gets the associated text tags.</summary>
    public IReadOnlyList<string> Tags => _tags;

#endregion

#region Private

    private JObject ToJson()
    {
        var obj = new JObject
        {
            ["id"] = Id,
            ["file_name"] = FileName,
            ["processing_info"] = (long)ProcessingInfos,
            ["width"] = Width,
            ["height"] = Height,
            ["orientation"] = (int)Orientation,
            ["title"] = Title,
            ["person_tags"] = new JArray(PersonTags.Select(pt => pt.ToJson())),
            ["tags"] = new JArray(Tags)
        };

        if (IsDateTimeSet)
        {
            obj["date_taken"] = DateTimeHelper.ToJson(DateTaken);
        }

        if (Location != null)
        {
            obj["location"] = Location.ToJson();
        }

        return obj;
    }

    private static Image FromJson(JObject jsonObject)
    {
        var jsonPersonTags = (JArray)jsonObject["person_tags"];
        var jsonTags = (JArray)jsonObject["tags"];
        return new(
            (long)jsonObject["id"],
            (string)jsonObject["file_name"],
            (ProcessingInfos)(long)jsonObject["processing_info"],
            (int)jsonObject["width"],
            (int)jsonObject["height"],
            (OrientationMode)(int)jsonObject["orientation"],
            DateTimeHelper.FromJson((JObject)jsonObject["date_taken"]),
            (string)jsonObject["title"],
            Location.FromJson((JObject)jsonObject["location"]),
            jsonPersonTags?.Select(v => PersonTag.FromJson((JObject)v)).ToArray(),
            jsonTags?.Select(v => (string)v).ToArray()
        );
    }

    private bool EqualsImp(Image other)
    {
        return Id == other.Id &&
               string.Equals(FileName, other.FileName) &&
               ProcessingInfos == other.ProcessingInfos &&
               Width == other.Width &&
               Height == other.Height &&
               Orientation == other.Orientation &&
               DateTaken.ToUnixTimeSeconds().Equals(other.DateTaken.ToUnixTimeSeconds()) &&
               string.Equals(Title, other.Title) &&
               Equals(Location, other.Location) &&
               _personTags.SequenceEqual(other._personTags) &&
               _tags.SequenceEqual(other._tags);
    }

    private readonly IReadOnlyList<PersonTag> _personTags = _personsTags ?? [];
    private readonly IReadOnlyList<string> _tags = _tags ?? [];

#endregion
}