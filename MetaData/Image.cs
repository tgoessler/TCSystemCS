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
//  see https://github.com/ThE-TiGeR/TCSystemCS for details.
//  Copyright (C) 2003 - 2024 Thomas Goessler. All Rights Reserved.
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

public sealed class Image(long fileId, string fileName, ProcessingInfos processingInfos, int width,
                          int height, OrientationMode orientation, DateTimeOffset dateTaken, string title,
                          Location location, IReadOnlyList<PersonTag> personsTags, IReadOnlyList<string> tags) : IEquatable<Image>
{
#region Public

    public bool HasPerson(string name)
    {
        return GetPersonTag(name) != null;
    }

    public bool HasPersonTag(PersonTag personTag)
    {
        return _personTags.FirstOrDefault(p => p.Equals(personTag)) != null;
    }

    public PersonTag GetPersonTag(string name)
    {
        return _personTags.FirstOrDefault(pt => pt.Person.Name == name);
    }

    public bool HasTag(string t)
    {
        return _tags.FirstOrDefault(x => x == t) != null;
    }

    public static Image ChangeFileName(Image image, string fileName)
    {
        return new(image.Id, fileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            image._personTags, image._tags);
    }

    public static Image ChangeDateTaken(Image image, DateTimeOffset dateTaken)
    {
        return new(image.Id, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            dateTaken, image.Title, image.Location,
            image._personTags, image._tags);
    }

    public static Image ChangeProcessingInfo(Image image, ProcessingInfos processingInfos)
    {
        return new(image.Id, image.FileName, processingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            image._personTags, image._tags);
    }

    public static Image ChangeLocation(Image image, Location location)
    {
        return new(image.Id, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, location,
            image._personTags, image._tags);
    }

    public static Image ChangePersonTags(Image image, IEnumerable<PersonTag> personTags)
    {
        return new(image.Id, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            personTags.ToArray(), image._tags);
    }

    public static Image ChangeTags(Image image, IEnumerable<string> tagsIn)
    {
        return new(image.Id, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            image._personTags, tagsIn.ToArray());
    }

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

    public static Image ChangePersonTagVisible(Image image, PersonTag pt, bool visible)
    {
        if (image.HasPersonTag(pt))
        {
            List<PersonTag> pts = image._personTags.ToList();
            pts.Remove(pt);
            pt = new(pt.Person, new(pt.Face.Id, pt.Face.Rectangle, pt.Face.FaceMode, visible, pt.Face.FaceDescriptor));
            pts.Add(pt);

            image = new(image.Id, image.FileName, image.ProcessingInfos,
                image.Width, image.Height, image.Orientation,
                image.DateTaken, image.Title, image.Location,
                pts, image._tags);
        }

        return image;
    }

    public static Image ChangePerson(Image image, Person person)
    {
        if (image.GetPersonTag(person.Name) is var pt)
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

    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as Image, EqualsImp);
    }

    public bool Equals(Image other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

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

    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    public static Image FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    public long Id { get; } = fileId;
    public string Name => FileName.Substring(FileName.LastIndexOf('\\') + 1);
    public string FileName { get; } = fileName;
    public ProcessingInfos ProcessingInfos { get; } = processingInfos;
    public int Width { get; } = width;
    public int Height { get; } = height;
    public OrientationMode Orientation { get; } = orientation;
    public DateTimeOffset DateTaken { get; } = dateTaken.Trim(TimeSpan.TicksPerSecond);
    public bool IsDateTimeSet => DateTaken != InvalidDateTaken;
    public static DateTimeOffset InvalidDateTaken { get; } = DateTimeOffset.FromUnixTimeSeconds(0).ToLocalTime();

    public string Title { get; } = title ?? "";
    public Location Location { get; } = location;
    public IReadOnlyList<PersonTag> PersonTags => _personTags;

    public int NumTags => _tags.Count;

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

    private readonly IReadOnlyList<PersonTag> _personTags = personsTags ?? [];
    private readonly IReadOnlyList<string> _tags = tags ?? [];

#endregion
}