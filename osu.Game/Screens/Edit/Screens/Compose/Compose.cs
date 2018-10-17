// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using OpenTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osu.Game.Rulesets.Objects;
using osu.Game.Screens.Edit.Screens.Compose.Timeline;

namespace osu.Game.Screens.Edit.Screens.Compose
{
    [Cached(Type = typeof(IPlacementHandler))]
    public class Compose : EditorScreen, IPlacementHandler
    {
        private const float vertical_margins = 10;
        private const float horizontal_margins = 20;

        /// <summary>
        /// Invoked when the placement of a <see cref="HitObject"/> has started.
        /// </summary>
        public event Action<HitObject> PlacementStarted;

        /// <summary>
        /// Invoked when the placement of a <see cref="HitObject"/> has finished.
        /// </summary>
        public event Action<HitObject> PlacementFinished;

        private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor();

        private Container composerContainer;

        [BackgroundDependencyLoader(true)]
        private void load([CanBeNull] BindableBeatDivisor beatDivisor)
        {
            if (beatDivisor != null)
                this.beatDivisor.BindTo(beatDivisor);

            Children = new Drawable[]
            {
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new Container
                            {
                                Name = "Timeline",
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Color4.Black.Opacity(0.5f)
                                    },
                                    new Container
                                    {
                                        Name = "Timeline content",
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding { Horizontal = horizontal_margins, Vertical = vertical_margins },
                                        Child = new GridContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Content = new[]
                                            {
                                                new Drawable[]
                                                {
                                                    new Container
                                                    {
                                                        RelativeSizeAxes = Axes.Both,
                                                        Padding = new MarginPadding { Right = 5 },
                                                        Child = new TimelineArea { RelativeSizeAxes = Axes.Both }
                                                    },
                                                    new BeatDivisorControl(beatDivisor) { RelativeSizeAxes = Axes.Both }
                                                },
                                            },
                                            ColumnDimensions = new[]
                                            {
                                                new Dimension(),
                                                new Dimension(GridSizeMode.Absolute, 90),
                                            }
                                        },
                                    }
                                }
                            }
                        },
                        new Drawable[]
                        {
                            composerContainer = new Container
                            {
                                Name = "Composer content",
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Horizontal = horizontal_margins, Vertical = vertical_margins },
                            }
                        }
                    },
                    RowDimensions = new[] { new Dimension(GridSizeMode.Absolute, 110) }
                },
            };

            var ruleset = Beatmap.Value.BeatmapInfo.Ruleset?.CreateInstance();
            if (ruleset == null)
            {
                Logger.Log("Beatmap doesn't have a ruleset assigned.");
                // ExitRequested?.Invoke();
                return;
            }

            var composer = ruleset.CreateHitObjectComposer();
            if (composer == null)
            {
                Logger.Log($"Ruleset {ruleset.Description} doesn't support hitobject composition.");
                // ExitRequested?.Invoke();
                return;
            }

            composerContainer.Child = composer;
        }

        public void BeginPlacement(HitObject hitObject) => PlacementStarted?.Invoke(hitObject);

        public void EndPlacement(HitObject hitObject) => PlacementFinished?.Invoke(hitObject);
    }
}
