using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Abstractions;
using PHD2Insight.Parser.Infrastructure;
using PHD2Insight.Parser.Internal;

namespace PHD2Insight.Parser.Parsers;

public sealed class GuideLogParser : IGuideLogParser {
    public ParseResult<GuideLog> Parse(Stream stream) {
        ArgumentNullException.ThrowIfNull(stream);

        var context = new GuideLogParseContext();

        using var reader = new LineReader(stream);

        while (reader.Read()) {
            var line = reader.CurrentLine!;

            if (VersionLineParser.TryParse(line, out var version)) {
                context.Version = version;
                continue;
            }

            if (SessionStartLineParser.TryParse(line, out var startTime)) {

                context.ClearContinuation();

                if (context.CurrentSession is not null) {
                    context.Sessions.Add(
                        context.CurrentSession.Build());
                }

                context.CurrentSession =
                    new GuidingSessionBuilder(startTime);

                continue;
            }

            if (SessionEndLineParser.TryParse(line, out var endTime)) {

                context.ClearContinuation();

                if (context.CurrentSession is not null) {
                    context.CurrentSession.Close(endTime);

                    context.Sessions.Add(
                        context.CurrentSession.Build());

                    context.CurrentSession = null;
                }

                continue;
            }

            if (EquipmentProfileLineParser.TryParse(line, out var profileName)) {

                context.ClearContinuation();

                if (context.CurrentSession is not null) {
                    context.CurrentSession.Equipment =
                        new EquipmentProfile {
                            Name = profileName!
                        };
                }

                continue;
            }

            if (ExposureLineParser.TryParse(
                                        line,
                                        out var exposureMs)) {

                context.ClearContinuation();

                if (context.CurrentSession is not null) {
                    context.CurrentSession.ExposureMilliseconds =
                        exposureMs;
                }

                continue;
            }

            if (PixelScaleLineParser.TryParse(
                                        line,
                                        out var info)) {

                context.ClearContinuation();

                context.CurrentSession!.PixelScale = info.PixelScale;
                context.CurrentSession.Binning = info.Binning;
                context.CurrentSession.FocalLengthMm = info.FocalLengthMm;

                continue;
            }

            if (CameraInfoLineParser.TryParse(line, out var cameraInfo)) {

                context.ClearContinuation();

                if (context.CurrentSession is not null) {
                    context.CurrentSession.Camera = cameraInfo;
                }
                continue;
            }

            if (MountInfoLineParser.TryParse(
                                        line,
                                        out var mount)) {

                context.ClearContinuation();

                context.CurrentSession!.Mount = mount;

                continue;
            }

            if (GuideAlgorithmLineParser.TryParse(
                    line,
                    out var axis,
                    out var algorithm)) {

                if (context.CurrentSession is not null) {
                    if (axis == GuideAxis.X) {
                        context.CurrentSession.xGuideAlgorithm = algorithm;
                        context.ContinuationMode = ContinuationMode.XGuideAlgorithm;
                    } else {
                        context.CurrentSession.yGuideAlgorithm = algorithm;
                        context.ContinuationMode = ContinuationMode.YGuideAlgorithm;
                    }
                }

                continue;
            }

            if (context.ContinuationMode == ContinuationMode.XGuideAlgorithm &&
    context.CurrentSession?.xGuideAlgorithm is not null) {
                if (GuideAlgorithmContinuationLineParser.TryApply(
                        line,
                        context.CurrentSession.xGuideAlgorithm)) {
                    continue;
                }
            }

            if (context.ContinuationMode == ContinuationMode.YGuideAlgorithm &&
                context.CurrentSession?.yGuideAlgorithm is not null) {
                if (GuideAlgorithmContinuationLineParser.TryApply(
                        line,
                        context.CurrentSession.yGuideAlgorithm)) {
                    continue;
                }
            }
        }

        return new ParseResult<GuideLog> {
            Success = true,
            Value = context.Build()
        };
    }
}