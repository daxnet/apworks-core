﻿// ==================================================================================================================
//        ,::i                                                           BBB
//       BBBBBi                                                         EBBB
//      MBBNBBU                                                         BBB,
//     BBB. BBB     BBB,BBBBM   BBB   UBBB   MBB,  LBBBBBO,   :BBG,BBB :BBB  .BBBU  kBBBBBF
//    BBB,  BBB    7BBBBS2BBBO  BBB  iBBBB  YBBJ :BBBMYNBBB:  FBBBBBB: OBB: 5BBB,  BBBi ,M,
//   MBBY   BBB.   8BBB   :BBB  BBB .BBUBB  BB1  BBBi   kBBB  BBBM     BBBjBBBr    BBB1
//  BBBBBBBBBBBu   BBB    FBBP  MBM BB. BB BBM  7BBB    MBBY .BBB     7BBGkBB1      JBBBBi
// PBBBFE0GkBBBB  7BBX   uBBB   MBBMBu .BBOBB   rBBB   kBBB  ZBBq     BBB: BBBJ   .   iBBB
//BBBB      iBBB  BBBBBBBBBE    EBBBB  ,BBBB     MBBBBBBBM   BBB,    iBBB  .BBB2 :BBBBBBB7
//vr7        777  BBBu8O5:      .77r    Lr7       .7EZk;     L77     .Y7r   irLY  JNMMF:
//               LBBj
//
// Apworks Application Development Framework
// Copyright (C) 2009-2018 by daxnet.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ==================================================================================================================

using System;

namespace EasyBooking.MeetingRooms.API.Models
{
    /// <summary>
    /// Represents the value object that indicates the location of the meeting room.
    /// </summary>
    public class Location
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the area on a certain floor where the meeting room resides.
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Gets or sets the name of the building in which the meeting room resides.
        /// </summary>
        public string Building { get; set; }

        /// <summary>
        /// Gets or sets the floor number on which the meeting room resides.
        /// </summary>
        public string Floor { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override bool Equals(object obj) => obj is Location location &&
                   Building == location.Building &&
                   Floor == location.Floor &&
                   Area == location.Area;

        public override int GetHashCode() => HashCode.Combine(Building, Floor, Area);

        public override string ToString() => $"{Area}, {Floor}, {Building}";

        #endregion Public Methods
    }
}