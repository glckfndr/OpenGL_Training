﻿namespace OpenGLHelper
{
    public static class TeapotData
    {
        public static int[,] patchdata = new int[,]
        {
            /* rim */
            {102, 103, 104, 105, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15},
            /* body */
            {12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27},
            {24, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40},
            /* lid */
            {96, 96, 96, 96, 97, 98, 99, 100, 101, 101, 101, 101, 0, 1, 2, 3,},
            {0, 1, 2, 3, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117},
            /* bottom */
            {118, 118, 118, 118, 124, 122, 119, 121, 123, 126, 125, 120, 40, 39, 38, 37},
            /* handle */
            {41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56},
            {53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 28, 65, 66, 67},
            /* spout */
            {68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83},
            {80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95}
        };

        public static float[,] cpdata = new float[,]
        {
            {0.2f, 0.0f, 2.7f},
            {0.2f, -0.112f, 2.7f},
            {0.112f, -0.2f, 2.7f},
            {0.0f, -0.2f, 2.7f},
            {1.3375f, 0.0f, 2.53125f},
            {1.3375f, -0.749f, 2.53125f},
            {0.749f, -1.3375f, 2.53125f},
            {0.0f, -1.3375f, 2.53125f},
            {1.4375f, 0.0f, 2.53125f},
            {1.4375f, -0.805f, 2.53125f},
            {0.805f, -1.4375f, 2.53125f},
            {0.0f, -1.4375f, 2.53125f},
            {1.5f, 0.0f, 2.4f},
            {1.5f, -0.84f, 2.4f},
            {0.84f, -1.5f, 2.4f},
            {0.0f, -1.5f, 2.4f},
            {1.75f, 0.0f, 1.875f},
            {1.75f, -0.98f, 1.875f},
            {0.98f, -1.75f, 1.875f},
            {0.0f, -1.75f, 1.875f},
            {2.0f, 0.0f, 1.35f},
            {2.0f, -1.12f, 1.35f},
            {1.12f, -2.0f, 1.35f},
            {0.0f, -2.0f, 1.35f},
            {2.0f, 0.0f, 0.9f},
            {2.0f, -1.12f, 0.9f},
            {1.12f, -2.0f, 0.9f},
            {0.0f, -2.0f, 0.9f},
            {-2.0f, 0.0f, 0.9f},
            {2.0f, 0.0f, 0.45f},
            {2.0f, -1.12f, 0.45f},
            {1.12f, -2.0f, 0.45f},
            {0.0f, -2.0f, 0.45f},
            {1.5f, 0.0f, 0.225f},
            {1.5f, -0.84f, 0.225f},
            {0.84f, -1.5f, 0.225f},
            {0.0f, -1.5f, 0.225f},
            {1.5f, 0.0f, 0.15f},
            {1.5f, -0.84f, 0.15f},
            {0.84f, -1.5f, 0.15f},
            {0.0f, -1.5f, 0.15f},
            {-1.6f, 0.0f, 2.025f},
            {-1.6f, -0.3f, 2.025f},
            {-1.5f, -0.3f, 2.25f},
            {-1.5f, 0.0f, 2.25f},
            {-2.3f, 0.0f, 2.025f},
            {-2.3f, -0.3f, 2.025f},
            {-2.5f, -0.3f, 2.25f},
            {-2.5f, 0.0f, 2.25f},
            {-2.7f, 0.0f, 2.025f},
            {-2.7f, -0.3f, 2.025f},
            {-3.0f, -0.3f, 2.25f},
            {-3.0f, 0.0f, 2.25f},
            {-2.7f, 0.0f, 1.8f},
            {-2.7f, -0.3f, 1.8f},
            {-3.0f, -0.3f, 1.8f},
            {-3.0f, 0.0f, 1.8f},
            {-2.7f, 0.0f, 1.575f},
            {-2.7f, -0.3f, 1.575f},
            {-3.0f, -0.3f, 1.35f},
            {-3.0f, 0.0f, 1.35f},
            {-2.5f, 0.0f, 1.125f},
            {-2.5f, -0.3f, 1.125f},
            {-2.65f, -0.3f, 0.9375f},
            {-2.65f, 0.0f, 0.9375f},
            {-2.0f, -0.3f, 0.9f},
            {-1.9f, -0.3f, 0.6f},
            {-1.9f, 0.0f, 0.6f},
            {1.7f, 0.0f, 1.425f},
            {1.7f, -0.66f, 1.425f},
            {1.7f, -0.66f, 0.6f},
            {1.7f, 0.0f, 0.6f},
            {2.6f, 0.0f, 1.425f},
            {2.6f, -0.66f, 1.425f},
            {3.1f, -0.66f, 0.825f},
            {3.1f, 0.0f, 0.825f},
            {2.3f, 0.0f, 2.1f},
            {2.3f, -0.25f, 2.1f},
            {2.4f, -0.25f, 2.025f},
            {2.4f, 0.0f, 2.025f},
            {2.7f, 0.0f, 2.4f},
            {2.7f, -0.25f, 2.4f},
            {3.3f, -0.25f, 2.4f},
            {3.3f, 0.0f, 2.4f},
            {2.8f, 0.0f, 2.475f},
            {2.8f, -0.25f, 2.475f},
            {3.525f, -0.25f, 2.49375f},
            {3.525f, 0.0f, 2.49375f},
            {2.9f, 0.0f, 2.475f},
            {2.9f, -0.15f, 2.475f},
            {3.45f, -0.15f, 2.5125f},
            {3.45f, 0.0f, 2.5125f},
            {2.8f, 0.0f, 2.4f},
            {2.8f, -0.15f, 2.4f},
            {3.2f, -0.15f, 2.4f},
            {3.2f, 0.0f, 2.4f},
            {0.0f, 0.0f, 3.15f},
            {0.8f, 0.0f, 3.15f},
            {0.8f, -0.45f, 3.15f},
            {0.45f, -0.8f, 3.15f},
            {0.0f, -0.8f, 3.15f},
            {0.0f, 0.0f, 2.85f},
            {1.4f, 0.0f, 2.4f},
            {1.4f, -0.784f, 2.4f},
            {0.784f, -1.4f, 2.4f},
            {0.0f, -1.4f, 2.4f},
            {0.4f, 0.0f, 2.55f},
            {0.4f, -0.224f, 2.55f},
            {0.224f, -0.4f, 2.55f},
            {0.0f, -0.4f, 2.55f},
            {1.3f, 0.0f, 2.55f},
            {1.3f, -0.728f, 2.55f},
            {0.728f, -1.3f, 2.55f},
            {0.0f, -1.3f, 2.55f},
            {1.3f, 0.0f, 2.4f},
            {1.3f, -0.728f, 2.4f},
            {0.728f, -1.3f, 2.4f},
            {0.0f, -1.3f, 2.4f},
            {0.0f, 0.0f, 0.0f},
            {1.425f, -0.798f, 0.0f},
            {1.5f, 0.0f, 0.075f},
            {1.425f, 0.0f, 0.0f},
            {0.798f, -1.425f, 0.0f},
            {0.0f, -1.5f, 0.075f},
            {0.0f, -1.425f, 0.0f},
            {1.5f, -0.84f, 0.075f},
            {0.84f, -1.5f, 0.075f}
        };
    }
}
