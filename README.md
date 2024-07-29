## v20240730
1. 抽象了通用存储类ISegmentsStore，结合跳表完成了默认实现，可在日后自由更换成其他实现方式而不影响IntensitySegments的使用；相对的，因为ISegmentsStore定义的方法比较通用，所以也可以用在IntensitySegments之外；
2. 移除了IntensitySegments.ToString()中的写/删除操作；
3. 屏蔽了SkipListNode，在SkipList Api中仅保留索引概念，最大化降低使用方的理解成本；

## v20240727
1. 编程语言为C#，总体思路使用跳表优化插入性能至O(logN)，N=线段端点数量。极端情况下可能会导致插入性能劣化至O(n)，可以通过类似线段树的lazy机制去除写入时对多段线段的实时更新；
2. 实际需要根据具体情况进行调整，考虑维度有：数据离散程度、读写调用次数比例等；