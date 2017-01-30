enabled(){  
  var ranges = [
    [ 0x203C, 0x203C ],
    [ 0x2049, 0x2049 ],
    [ 0x2139, 0x2139 ],
    [ 0x2194, 0x2199 ],
    [ 0x21A9, 0x21AA ],
    [ 0x231A, 0x231B ],
    [ 0x2328, 0x2328 ],
    [ 0x23CF, 0x23CF ],
    [ 0x23E9, 0x23F3 ],
    [ 0x23F8, 0x23FA ],
    [ 0x24C2, 0x24C2 ],
    [ 0x25AA, 0x25AB ],
    [ 0x25B6, 0x25B6 ],
    [ 0x25C0, 0x25C0 ],
    [ 0x25FB, 0x25FE ],
    [ 0x2600, 0x2604 ],
    [ 0x260E, 0x260E ],
    [ 0x2611, 0x2611 ],
    [ 0x2614, 0x2615 ],
    [ 0x2618, 0x2618 ],
    [ 0x261D, 0x261D ],
    [ 0x2620, 0x2620 ],
    [ 0x2622, 0x2623 ],
    [ 0x2626, 0x2626 ],
    [ 0x262A, 0x262A ],
    [ 0x262E, 0x262F ],
    [ 0x2638, 0x263A ],
    [ 0x2640, 0x2640 ],
    [ 0x2642, 0x2642 ],
    [ 0x2648, 0x2653 ],
    [ 0x2660, 0x2660 ],
    [ 0x2663, 0x2663 ],
    [ 0x2665, 0x2666 ],
    [ 0x2668, 0x2668 ],
    [ 0x267B, 0x267B ],
    [ 0x267F, 0x267F ],
    [ 0x2692, 0x2697 ],
    [ 0x2699, 0x2699 ],
    [ 0x269B, 0x269C ],
    [ 0x26A0, 0x26A1 ],
    [ 0x26AA, 0x26AB ],
    [ 0x26B0, 0x26B1 ],
    [ 0x26BD, 0x26BE ],
    [ 0x26C4, 0x26C5 ],
    [ 0x26C8, 0x26C8 ],
    [ 0x26CE, 0x26CF ],
    [ 0x26D1, 0x26D1 ],
    [ 0x26D3, 0x26D4 ],
    [ 0x26E9, 0x26EA ],
    [ 0x26F0, 0x26F5 ],
    [ 0x26F7, 0x26FA ],
    [ 0x26FD, 0x26FD ],
    [ 0x2702, 0x2702 ],
    [ 0x2705, 0x2705 ],
    [ 0x2708, 0x270D ],
    [ 0x270F, 0x270F ],
    [ 0x2712, 0x2712 ],
    [ 0x2714, 0x2714 ],
    [ 0x2716, 0x2716 ],
    [ 0x271D, 0x271D ],
    [ 0x2721, 0x2721 ],
    [ 0x2728, 0x2728 ],
    [ 0x2733, 0x2734 ],
    [ 0x2744, 0x2744 ],
    [ 0x2747, 0x2747 ],
    [ 0x274C, 0x274C ],
    [ 0x274E, 0x274E ],
    [ 0x2753, 0x2755 ],
    [ 0x2757, 0x2757 ],
    [ 0x2763, 0x2764 ],
    [ 0x2795, 0x2797 ],
    [ 0x27A1, 0x27A1 ],
    [ 0x27B0, 0x27B0 ],
    [ 0x27BF, 0x27BF ],
    [ 0x2934, 0x2935 ],
    [ 0x2B05, 0x2B07 ],
    [ 0x2B1B, 0x2B1C ],
    [ 0x2B50, 0x2B50 ],
    [ 0x2B55, 0x2B55 ],
    [ 0x3030, 0x3030 ],
    [ 0x303D, 0x303D ],
    [ 0x3297, 0x3297 ],
    [ 0x3299, 0x3299 ],
    [ 0xE50A, 0xE50A ],
    [ 0x10000, 0x10000 ],
    [ 0x1F004, 0x1F004 ],
    [ 0x1F0CF, 0x1F0CF ],
    [ 0x1F170, 0x1F171 ],
    [ 0x1F17E, 0x1F17F ],
    [ 0x1F18E, 0x1F18E ],
    [ 0x1F191, 0x1F19A ],
    [ 0x1F1E6, 0x1F1FF ],
    [ 0x1F201, 0x1F202 ],
    [ 0x1F21A, 0x1F21A ],
    [ 0x1F22F, 0x1F22F ],
    [ 0x1F232, 0x1F23A ],
    [ 0x1F250, 0x1F251 ],
    [ 0x1F300, 0x1F321 ],
    [ 0x1F324, 0x1F393 ],
    [ 0x1F396, 0x1F397 ],
    [ 0x1F399, 0x1F39B ],
    [ 0x1F39E, 0x1F3F0 ],
    [ 0x1F3F3, 0x1F3F5 ],
    [ 0x1F3F7, 0x1F4FD ],
    [ 0x1F4FF, 0x1F53D ],
    [ 0x1F549, 0x1F54E ],
    [ 0x1F550, 0x1F567 ],
    [ 0x1F56F, 0x1F570 ],
    [ 0x1F573, 0x1F57A ],
    [ 0x1F587, 0x1F587 ],
    [ 0x1F58A, 0x1F58D ],
    [ 0x1F590, 0x1F590 ],
    [ 0x1F595, 0x1F596 ],
    [ 0x1F5A4, 0x1F5A5 ],
    [ 0x1F5A8, 0x1F5A8 ],
    [ 0x1F5B1, 0x1F5B2 ],
    [ 0x1F5BC, 0x1F5BC ],
    [ 0x1F5C2, 0x1F5C4 ],
    [ 0x1F5D1, 0x1F5D3 ],
    [ 0x1F5DC, 0x1F5DE ],
    [ 0x1F5E1, 0x1F5E1 ],
    [ 0x1F5E3, 0x1F5E3 ],
    [ 0x1F5E8, 0x1F5E8 ],
    [ 0x1F5EF, 0x1F5EF ],
    [ 0x1F5F3, 0x1F5F3 ],
    [ 0x1F5FA, 0x1F64F ],
    [ 0x1F680, 0x1F6C5 ],
    [ 0x1F6CB, 0x1F6D2 ],
    [ 0x1F6E0, 0x1F6E5 ],
    [ 0x1F6E9, 0x1F6E9 ],
    [ 0x1F6EB, 0x1F6EC ],
    [ 0x1F6F0, 0x1F6F0 ],
    [ 0x1F6F3, 0x1F6F6 ],
    [ 0x1F910, 0x1F91E ],
    [ 0x1F920, 0x1F927 ],
    [ 0x1F930, 0x1F930 ],
    [ 0x1F933, 0x1F93A ],
    [ 0x1F93C, 0x1F93E ],
    [ 0x1F940, 0x1F945 ],
    [ 0x1F947, 0x1F94B ],
    [ 0x1F950, 0x1F95E ],
    [ 0x1F980, 0x1F991 ],
    [ 0x1F9C0, 0x1F9C0 ]
  ];
  
  var combined = [ // list of combined + extra emoji taken from https://github.com/iamcal/js-emoji
    "\u0023\uFE0F\u20E3",
    "\u002A\u20E3",
    "\u0030\uFE0F\u20E3",
    "\u0031\uFE0F\u20E3",
    "\u0032\uFE0F\u20E3",
    "\u0033\uFE0F\u20E3",
    "\u0034\uFE0F\u20E3",
    "\u0035\uFE0F\u20E3",
    "\u0036\uFE0F\u20E3",
    "\u0037\uFE0F\u20E3",
    "\u0038\uFE0F\u20E3",
    "\u0039\uFE0F\u20E3",
    "\uD83C\uDDE6\uD83C\uDDE8",
    "\uD83C\uDDE6\uD83C\uDDE9",
    "\uD83C\uDDE6\uD83C\uDDEA",
    "\uD83C\uDDE6\uD83C\uDDEB",
    "\uD83C\uDDE6\uD83C\uDDEC",
    "\uD83C\uDDE6\uD83C\uDDEE",
    "\uD83C\uDDE6\uD83C\uDDF1",
    "\uD83C\uDDE6\uD83C\uDDF2",
    "\uD83C\uDDE6\uD83C\uDDF4",
    "\uD83C\uDDE6\uD83C\uDDF6",
    "\uD83C\uDDE6\uD83C\uDDF7",
    "\uD83C\uDDE6\uD83C\uDDF8",
    "\uD83C\uDDE6\uD83C\uDDF9",
    "\uD83C\uDDE6\uD83C\uDDFA",
    "\uD83C\uDDE6\uD83C\uDDFC",
    "\uD83C\uDDE6\uD83C\uDDFD",
    "\uD83C\uDDE6\uD83C\uDDFF",
    "\uD83C\uDDE7\uD83C\uDDE6",
    "\uD83C\uDDE7\uD83C\uDDE7",
    "\uD83C\uDDE7\uD83C\uDDE9",
    "\uD83C\uDDE7\uD83C\uDDEA",
    "\uD83C\uDDE7\uD83C\uDDEB",
    "\uD83C\uDDE7\uD83C\uDDEC",
    "\uD83C\uDDE7\uD83C\uDDED",
    "\uD83C\uDDE7\uD83C\uDDEE",
    "\uD83C\uDDE7\uD83C\uDDEF",
    "\uD83C\uDDE7\uD83C\uDDF1",
    "\uD83C\uDDE7\uD83C\uDDF2",
    "\uD83C\uDDE7\uD83C\uDDF3",
    "\uD83C\uDDE7\uD83C\uDDF4",
    "\uD83C\uDDE7\uD83C\uDDF6",
    "\uD83C\uDDE7\uD83C\uDDF7",
    "\uD83C\uDDE7\uD83C\uDDF8",
    "\uD83C\uDDE7\uD83C\uDDF9",
    "\uD83C\uDDE7\uD83C\uDDFB",
    "\uD83C\uDDE7\uD83C\uDDFC",
    "\uD83C\uDDE7\uD83C\uDDFE",
    "\uD83C\uDDE7\uD83C\uDDFF",
    "\uD83C\uDDE8\uD83C\uDDE6",
    "\uD83C\uDDE8\uD83C\uDDE8",
    "\uD83C\uDDE8\uD83C\uDDE9",
    "\uD83C\uDDE8\uD83C\uDDEB",
    "\uD83C\uDDE8\uD83C\uDDEC",
    "\uD83C\uDDE8\uD83C\uDDED",
    "\uD83C\uDDE8\uD83C\uDDEE",
    "\uD83C\uDDE8\uD83C\uDDF0",
    "\uD83C\uDDE8\uD83C\uDDF1",
    "\uD83C\uDDE8\uD83C\uDDF2",
    "\uD83C\uDDE8\uD83C\uDDF3",
    "\uD83C\uDDE8\uD83C\uDDF4",
    "\uD83C\uDDE8\uD83C\uDDF5",
    "\uD83C\uDDE8\uD83C\uDDF7",
    "\uD83C\uDDE8\uD83C\uDDFA",
    "\uD83C\uDDE8\uD83C\uDDFB",
    "\uD83C\uDDE8\uD83C\uDDFC",
    "\uD83C\uDDE8\uD83C\uDDFD",
    "\uD83C\uDDE8\uD83C\uDDFE",
    "\uD83C\uDDE8\uD83C\uDDFF",
    "\uD83C\uDDE9\uD83C\uDDEA",
    "\uD83C\uDDE9\uD83C\uDDEC",
    "\uD83C\uDDE9\uD83C\uDDEF",
    "\uD83C\uDDE9\uD83C\uDDF0",
    "\uD83C\uDDE9\uD83C\uDDF2",
    "\uD83C\uDDE9\uD83C\uDDF4",
    "\uD83C\uDDE9\uD83C\uDDFF",
    "\uD83C\uDDEA\uD83C\uDDE6",
    "\uD83C\uDDEA\uD83C\uDDE8",
    "\uD83C\uDDEA\uD83C\uDDEA",
    "\uD83C\uDDEA\uD83C\uDDEC",
    "\uD83C\uDDEA\uD83C\uDDED",
    "\uD83C\uDDEA\uD83C\uDDF7",
    "\uD83C\uDDEA\uD83C\uDDF8",
    "\uD83C\uDDEA\uD83C\uDDF9",
    "\uD83C\uDDEA\uD83C\uDDFA",
    "\uD83C\uDDEB\uD83C\uDDEE",
    "\uD83C\uDDEB\uD83C\uDDEF",
    "\uD83C\uDDEB\uD83C\uDDF0",
    "\uD83C\uDDEB\uD83C\uDDF2",
    "\uD83C\uDDEB\uD83C\uDDF4",
    "\uD83C\uDDEB\uD83C\uDDF7",
    "\uD83C\uDDEC\uD83C\uDDE6",
    "\uD83C\uDDEC\uD83C\uDDE7",
    "\uD83C\uDDEC\uD83C\uDDE9",
    "\uD83C\uDDEC\uD83C\uDDEA",
    "\uD83C\uDDEC\uD83C\uDDEB",
    "\uD83C\uDDEC\uD83C\uDDEC",
    "\uD83C\uDDEC\uD83C\uDDED",
    "\uD83C\uDDEC\uD83C\uDDEE",
    "\uD83C\uDDEC\uD83C\uDDF1",
    "\uD83C\uDDEC\uD83C\uDDF2",
    "\uD83C\uDDEC\uD83C\uDDF3",
    "\uD83C\uDDEC\uD83C\uDDF5",
    "\uD83C\uDDEC\uD83C\uDDF6",
    "\uD83C\uDDEC\uD83C\uDDF7",
    "\uD83C\uDDEC\uD83C\uDDF8",
    "\uD83C\uDDEC\uD83C\uDDF9",
    "\uD83C\uDDEC\uD83C\uDDFA",
    "\uD83C\uDDEC\uD83C\uDDFC",
    "\uD83C\uDDEC\uD83C\uDDFE",
    "\uD83C\uDDED\uD83C\uDDF0",
    "\uD83C\uDDED\uD83C\uDDF2",
    "\uD83C\uDDED\uD83C\uDDF3",
    "\uD83C\uDDED\uD83C\uDDF7",
    "\uD83C\uDDED\uD83C\uDDF9",
    "\uD83C\uDDED\uD83C\uDDFA",
    "\uD83C\uDDEE\uD83C\uDDE8",
    "\uD83C\uDDEE\uD83C\uDDE9",
    "\uD83C\uDDEE\uD83C\uDDEA",
    "\uD83C\uDDEE\uD83C\uDDF1",
    "\uD83C\uDDEE\uD83C\uDDF2",
    "\uD83C\uDDEE\uD83C\uDDF3",
    "\uD83C\uDDEE\uD83C\uDDF4",
    "\uD83C\uDDEE\uD83C\uDDF6",
    "\uD83C\uDDEE\uD83C\uDDF7",
    "\uD83C\uDDEE\uD83C\uDDF8",
    "\uD83C\uDDEE\uD83C\uDDF9",
    "\uD83C\uDDEF\uD83C\uDDEA",
    "\uD83C\uDDEF\uD83C\uDDF2",
    "\uD83C\uDDEF\uD83C\uDDF4",
    "\uD83C\uDDEF\uD83C\uDDF5",
    "\uD83C\uDDF0\uD83C\uDDEA",
    "\uD83C\uDDF0\uD83C\uDDEC",
    "\uD83C\uDDF0\uD83C\uDDED",
    "\uD83C\uDDF0\uD83C\uDDEE",
    "\uD83C\uDDF0\uD83C\uDDF2",
    "\uD83C\uDDF0\uD83C\uDDF3",
    "\uD83C\uDDF0\uD83C\uDDF5",
    "\uD83C\uDDF0\uD83C\uDDF7",
    "\uD83C\uDDF0\uD83C\uDDFC",
    "\uD83C\uDDF0\uD83C\uDDFE",
    "\uD83C\uDDF0\uD83C\uDDFF",
    "\uD83C\uDDF1\uD83C\uDDE6",
    "\uD83C\uDDF1\uD83C\uDDE7",
    "\uD83C\uDDF1\uD83C\uDDE8",
    "\uD83C\uDDF1\uD83C\uDDEE",
    "\uD83C\uDDF1\uD83C\uDDF0",
    "\uD83C\uDDF1\uD83C\uDDF7",
    "\uD83C\uDDF1\uD83C\uDDF8",
    "\uD83C\uDDF1\uD83C\uDDF9",
    "\uD83C\uDDF1\uD83C\uDDFA",
    "\uD83C\uDDF1\uD83C\uDDFB",
    "\uD83C\uDDF1\uD83C\uDDFE",
    "\uD83C\uDDF2\uD83C\uDDE6",
    "\uD83C\uDDF2\uD83C\uDDE8",
    "\uD83C\uDDF2\uD83C\uDDE9",
    "\uD83C\uDDF2\uD83C\uDDEA",
    "\uD83C\uDDF2\uD83C\uDDEB",
    "\uD83C\uDDF2\uD83C\uDDEC",
    "\uD83C\uDDF2\uD83C\uDDED",
    "\uD83C\uDDF2\uD83C\uDDF0",
    "\uD83C\uDDF2\uD83C\uDDF1",
    "\uD83C\uDDF2\uD83C\uDDF2",
    "\uD83C\uDDF2\uD83C\uDDF3",
    "\uD83C\uDDF2\uD83C\uDDF4",
    "\uD83C\uDDF2\uD83C\uDDF5",
    "\uD83C\uDDF2\uD83C\uDDF6",
    "\uD83C\uDDF2\uD83C\uDDF7",
    "\uD83C\uDDF2\uD83C\uDDF8",
    "\uD83C\uDDF2\uD83C\uDDF9",
    "\uD83C\uDDF2\uD83C\uDDFA",
    "\uD83C\uDDF2\uD83C\uDDFB",
    "\uD83C\uDDF2\uD83C\uDDFC",
    "\uD83C\uDDF2\uD83C\uDDFD",
    "\uD83C\uDDF2\uD83C\uDDFE",
    "\uD83C\uDDF2\uD83C\uDDFF",
    "\uD83C\uDDF3\uD83C\uDDE6",
    "\uD83C\uDDF3\uD83C\uDDE8",
    "\uD83C\uDDF3\uD83C\uDDEA",
    "\uD83C\uDDF3\uD83C\uDDEB",
    "\uD83C\uDDF3\uD83C\uDDEC",
    "\uD83C\uDDF3\uD83C\uDDEE",
    "\uD83C\uDDF3\uD83C\uDDF1",
    "\uD83C\uDDF3\uD83C\uDDF4",
    "\uD83C\uDDF3\uD83C\uDDF5",
    "\uD83C\uDDF3\uD83C\uDDF7",
    "\uD83C\uDDF3\uD83C\uDDFA",
    "\uD83C\uDDF3\uD83C\uDDFF",
    "\uD83C\uDDF4\uD83C\uDDF2",
    "\uD83C\uDDF5\uD83C\uDDE6",
    "\uD83C\uDDF5\uD83C\uDDEA",
    "\uD83C\uDDF5\uD83C\uDDEB",
    "\uD83C\uDDF5\uD83C\uDDEC",
    "\uD83C\uDDF5\uD83C\uDDED",
    "\uD83C\uDDF5\uD83C\uDDF0",
    "\uD83C\uDDF5\uD83C\uDDF1",
    "\uD83C\uDDF5\uD83C\uDDF2",
    "\uD83C\uDDF5\uD83C\uDDF3",
    "\uD83C\uDDF5\uD83C\uDDF7",
    "\uD83C\uDDF5\uD83C\uDDF8",
    "\uD83C\uDDF5\uD83C\uDDF9",
    "\uD83C\uDDF5\uD83C\uDDFC",
    "\uD83C\uDDF5\uD83C\uDDFE",
    "\uD83C\uDDF6\uD83C\uDDE6",
    "\uD83C\uDDF7\uD83C\uDDEA",
    "\uD83C\uDDF7\uD83C\uDDF4",
    "\uD83C\uDDF7\uD83C\uDDF8",
    "\uD83C\uDDF7\uD83C\uDDFA",
    "\uD83C\uDDF7\uD83C\uDDFC",
    "\uD83C\uDDF8\uD83C\uDDE6",
    "\uD83C\uDDF8\uD83C\uDDE7",
    "\uD83C\uDDF8\uD83C\uDDE8",
    "\uD83C\uDDF8\uD83C\uDDE9",
    "\uD83C\uDDF8\uD83C\uDDEA",
    "\uD83C\uDDF8\uD83C\uDDEC",
    "\uD83C\uDDF8\uD83C\uDDED",
    "\uD83C\uDDF8\uD83C\uDDEE",
    "\uD83C\uDDF8\uD83C\uDDEF",
    "\uD83C\uDDF8\uD83C\uDDF0",
    "\uD83C\uDDF8\uD83C\uDDF1",
    "\uD83C\uDDF8\uD83C\uDDF2",
    "\uD83C\uDDF8\uD83C\uDDF3",
    "\uD83C\uDDF8\uD83C\uDDF4",
    "\uD83C\uDDF8\uD83C\uDDF7",
    "\uD83C\uDDF8\uD83C\uDDF8",
    "\uD83C\uDDF8\uD83C\uDDF9",
    "\uD83C\uDDF8\uD83C\uDDFB",
    "\uD83C\uDDF8\uD83C\uDDFD",
    "\uD83C\uDDF8\uD83C\uDDFE",
    "\uD83C\uDDF8\uD83C\uDDFF",
    "\uD83C\uDDF9\uD83C\uDDE6",
    "\uD83C\uDDF9\uD83C\uDDE8",
    "\uD83C\uDDF9\uD83C\uDDE9",
    "\uD83C\uDDF9\uD83C\uDDEB",
    "\uD83C\uDDF9\uD83C\uDDEC",
    "\uD83C\uDDF9\uD83C\uDDED",
    "\uD83C\uDDF9\uD83C\uDDEF",
    "\uD83C\uDDF9\uD83C\uDDF0",
    "\uD83C\uDDF9\uD83C\uDDF1",
    "\uD83C\uDDF9\uD83C\uDDF2",
    "\uD83C\uDDF9\uD83C\uDDF3",
    "\uD83C\uDDF9\uD83C\uDDF4",
    "\uD83C\uDDF9\uD83C\uDDF7",
    "\uD83C\uDDF9\uD83C\uDDF9",
    "\uD83C\uDDF9\uD83C\uDDFB",
    "\uD83C\uDDF9\uD83C\uDDFC",
    "\uD83C\uDDF9\uD83C\uDDFF",
    "\uD83C\uDDFA\uD83C\uDDE6",
    "\uD83C\uDDFA\uD83C\uDDEC",
    "\uD83C\uDDFA\uD83C\uDDF2",
    "\uD83C\uDDFA\uD83C\uDDF8",
    "\uD83C\uDDFA\uD83C\uDDFE",
    "\uD83C\uDDFA\uD83C\uDDFF",
    "\uD83C\uDDFB\uD83C\uDDE6",
    "\uD83C\uDDFB\uD83C\uDDE8",
    "\uD83C\uDDFB\uD83C\uDDEA",
    "\uD83C\uDDFB\uD83C\uDDEC",
    "\uD83C\uDDFB\uD83C\uDDEE",
    "\uD83C\uDDFB\uD83C\uDDF3",
    "\uD83C\uDDFB\uD83C\uDDFA",
    "\uD83C\uDDFC\uD83C\uDDEB",
    "\uD83C\uDDFC\uD83C\uDDF8",
    "\uD83C\uDDFD\uD83C\uDDF0",
    "\uD83C\uDDFE\uD83C\uDDEA",
    "\uD83C\uDDFE\uD83C\uDDF9",
    "\uD83C\uDDFF\uD83C\uDDE6",
    "\uD83C\uDDFF\uD83C\uDDF2",
    "\uD83C\uDDFF\uD83C\uDDFC",
    "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC66",
    "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC66\u200D\uD83D\uDC66",
    "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC67",
    "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC67\u200D\uD83D\uDC66",
    "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC67\u200D\uD83D\uDC67",
    "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC66\u200D\uD83D\uDC66",
    "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC67",
    "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC66",
    "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC67",
    "\uD83D\uDC68\u200D\u2764\uFE0F\u200D\uD83D\uDC68",
    "\uD83D\uDC68\u200D\u2764\uFE0F\u200D\uD83D\uDC8B\u200D\uD83D\uDC68",
    "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC66",
    "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC66\u200D\uD83D\uDC66",
    "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC67",
    "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC66",
    "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC67",
    "\uD83D\uDC69\u200D\u2764\uFE0F\u200D\uD83D\uDC69",
    "\uD83D\uDC69\u200D\u2764\uFE0F\u200D\uD83D\uDC8B\u200D\uD83D\uDC69"
  ];
  
  this.convUnicode = function(codePt){
    if (codePt > 0xFFFF){
      codePt -= 0x10000;
      return String.fromCharCode(0xD800+(codePt>>10), 0xDC00+(codePt&0x3FF));
    }
    else{
      return String.fromCharCode(codePt);
    }
  };
  
  // HTML generation
  
  var generated = [];
  
  ranges.forEach(range => {
    for(var index = range[0]; index <= range[1]; index++){
      generated.push(TD.util.cleanWithEmoji(this.convUnicode(index)));
    }
  });
  
  combined.forEach(str => {
    generated.push(TD.util.cleanWithEmoji(str));
  });
  
  this.emojiHTML = generated.join("");
  
  // styles
  
  this.css = window.TDPF_createCustomStyle(this);
  this.css.insert(".emoji-keyboard { position: absolute; width: 16.5em; height: 11.2em; background-color: white; overflow-y: auto; padding: 0.1em; box-sizing: border-box; border-radius: 2px; font-size: 24px; z-index: 9999 }");
  this.css.insert(".emoji-keyboard .emoji { padding: 0.1em !important; cursor: pointer }");
  
  // keyboard generation
  
  this.generateKeyboardFor = (input, left, top) => {
    var created = document.createElement("div");
    document.body.appendChild(created);
    
    created.classList.add("emoji-keyboard");
    created.style.left = left+"px";
    created.style.top = top+"px";
    created.innerHTML = this.emojiHTML;
    
    created.addEventListener("click", function(e){
      if (e.target.tagName === "IMG"){
        input.val(input.val()+e.target.getAttribute("alt"));
        input.trigger("change");
        input.focus();
      }
    });
    
    return created;
  };
  
  /*
   * TODO
   * ----
   * add copyright and trademark symbols manually
   * add emoji search if I can be bothered
   * figure out how to make emojis work properly in the textarea
   * lazy emoji loading
   */
  
  // injection
  
  this.prevComposeMustache = TD.mustaches["compose/docked_compose.mustache"];
  TD.mustaches["compose/docked_compose.mustache"] = TD.mustaches["compose/docked_compose.mustache"].replace('<div class="cf margin-t--12 margin-b--30">', '<div class="cf margin-t--12 margin-b--30"><button class="needsclick btn btn-on-blue txt-left margin-b--12 padding-v--9 emoji-keyboard-popup-btn"><i class="icon icon-heart"></i></button>');
  
  this.currentKeyboard = null;
  
  var me = this;
  
  this.emojiKeyboardButtonEvent = function(){
    if (me.currentKeyboard){
      $(me.currentKeyboard).remove();
      me.currentKeyboard = null;
      
      $(this).removeClass("is-selected");
      $(this).blur();
    }
    else{
      var pos = $(this).offset();
      me.currentKeyboard = me.generateKeyboardFor($(".js-compose-text").first(), pos.left, pos.top+$(this).outerHeight()+8);
      
      $(this).addClass("is-selected");
      $(this).blur();
    }
  };
}

ready(){
  $(".emoji-keyboard-popup-btn").on("click", this.emojiKeyboardButtonEvent);
}

disabled(){
  this.css.remove();
  
  $(".emoji-keyboard-popup-btn").off("click", this.emojiKeyboardButtonEvent);
  TD.mustaches["compose/docked_compose.mustache"] = this.prevComposeMustache;
}

/* use this to generate the 'ranges' array
(function(){
  var arrayText = "";

  var prevMatched = false;
  var prevStarted;

  for(var index = 0x2000; index <= 0x1F9CF; index++){
    var conv = this.convUnicode(index);

    if (TD.util.cleanWithEmoji(conv) != conv){
      if (!prevMatched){
        prevMatched = true;
        prevStarted = index;
      }
    }
    else{
      if (prevMatched){
        prevMatched = false;
        arrayText += "[ 0x"+prevStarted.toString(16).toUpperCase()+", 0x"+(index-1).toString(16).toUpperCase()+" ],\n";
      }
    }
  }

  console.info(arrayText);
})()
*/
