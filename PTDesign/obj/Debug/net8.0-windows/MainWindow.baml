   M S B A M L   `   `   `  ����C  ?PTDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:PTDesignPTDesign  5   	   3clr-namespace:PTDesign.LibraryPTDesign.Library  5   	   -clr-namespace:PTDesign.ViewPTDesign.View  5
   	       PTDesign.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PTDesign     5   	   2libraryclr-namespace:PTDesign.Library     5   	   _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  5	   	   $ucclr-namespace:PTDesign.View  5
   	     � WindowStyle$
  None=�5   	   $	��550��5   	   $	��500��6   ���	#FFEAEAEA � Title$ 
MainWindow��5   	    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $T Npack://application:,,,/PTDesign;component/Resource/Dictionary/StyleWindow.xaml?�6%   6   %    5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��25 �6   �� 5      $��* �6   .+�5   
   "   PTDesign.View.ControlBarView  $��0��6   6+   o� 5   
   $��5,0,0,0q�6   $��1��6    
������ .@�5      m� $
��File��6   .@�5      m� $��Xuất File��6    m� Command 6.   �� *
ExportData+5       m� 5!      $��Diễn giải��6   6,   m� 5"      $��
Tác giả��6   6*   5#      � 5%   
   $��2��6   .+�5&      � $��0��6   5(      � 5)       5*      �� 5+      $	��400 �6$   �� 5,      $��75 �6$   .+�5.      �� $��0,0,0,10q�6   $��1��6/    �� Orientation$ 
Horizontal=�6=   $��Center=�6V   .+�5/      �� $��80��6B      Primary Button#��[  6M   $��15,0,0,0q�6v   ��6   �� *ConnectAPIEtabs+.��6�   Kết nối API6�   �� 50      $��80��6   #��[  6)   $��	15,0,15,0q�6Q   ��51      �� *GetSlectedTendon+.��6&   Chọn cáp60   �� 52      $��0,0,10,0q�6   $��80��6/   #��[  6:   ��6b   �� *Check+.��6|   Cập nhập6�   �� 54      $��0,0,10,0q�6   $��80��6/   #��[  6:   ��6b   �� *AssignLoadTendonV1+.��6�   Gán tải V16�   57      �� 58      $��0��6   $��10,-1,10,10q�6*   .@�59      �� $
��Cáp��6   $��75��6+     TabItemStyle#��[ 5:   
   .��5;      �  5<      �� 5=   "   $	��100 �60   6?   �� 5>   "   63   .+�5@      � $��0��6$    � ColumnDefinitions 5A   "   �� 5B   &   $
��auto �67   6F   �� 5C   &   $��* �67   6C   �� 5D   &   $
��auto �67   6F   �� 5E   &   $��* �67   6C    5G   "   �� 5H   &   66   �� 5I   &   66   �� 5J   &   66   .+�5L   "   �� $��10,0,0,0q�6-   $��
Tên cáp ��6?     Primary TextBlock#��[ 6O   $��20��6{   6�   �� 5M   "   $��1��6+   	 �� DisplayMemberPath$
	 Name��6^     Primary Combobox#��[ 5N   6   $��95��6`   $��10,0,0,0q�6k   ��5M   ;   �� *
ListTendon+
 �� SelectedItem
 5N      �� *SelectedTendon+ �� Mode$ TwoWay=�6   �� 5O   "   $��2��6,   $��Section beam��6=   #��[ 6Q   $��20��6}   6�   �� 5P   "   $��3��6+   #��[ 5Q   5   $��95��6_   $��10,0,0,0q�6j   ��5P   <   �� *FrameProperties+
 5Q      �� *SelectedFrame+$ TwoWay=�6~   �� 5R   "   $��10,0,0,0q�6,   $��1��6>   $��P��6K   #��[ 6T   $��20��6�   6�   �� 5S   "   $��1��6*   $��1��6:   $��10,0,0,0q�6G   $��95��6Z     Primary TextBox#��[ 6e   $��20��6�   ��6�   �� *Force+$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5U   "   $��2��6,   $��1��6<   $��Load Pattern��6J   #��[ 6^   $��20��6�   6�   �� 5V   "   $��3��6+   $��1��6<   #��[ 5W   =   $��95��6g   $��10,0,0,0q�6r   ��5V   I   �� *LoadPatternList+
 5W      �� *SelectedLoadPattern+$ TwoWay=�6�   5X      &   System.Windows.Controls.DataGrid  5Y      $��1��6'      AutoGenerateColumns$ False��5[         CanUserAddRows$ False��5\         CanUserDeleteRows$ False��5]         GridLinesVisibility$	 All=�5^      ���	#FFB1B1B1
�������
���   �$��Stretch=�5b      $��Stretch=�5c         
IsReadOnly$ False��5d      ��5Z      �� *" SelectedTendon.ListTendonProfile+   Columns 5e   "   4  .System.Windows.Controls.DataGridTemplateColumn  5g   &   ,  &System.Windows.Controls.DataGridColumn   Header$ 
Sợi cáp��6=      Width5  /System.Windows.Controls.DataGridLengthConverter$ 2* 6N      CellTemplate 5h   *   �� 5i   .   .R�5j   2   �� ��6<   �� *
TendonName+5k   /      CellEditingTemplate 5m   *   �� 5n   .   .R�5o   2   �� >   8PTDesign.Library.TextBoxUpdateOnEnterOrLostFocusBehavior   Enable$
 True��5p   &   ��5o   :   �� *
TendonName+$ TwoWay=�$ Explicit=�5p   &   5q   /   5s   '     5t   &   $ A(mm)��6=   $ 2* 6L    5u   *   �� 5v   .   .R�5w   2   ��  �� TextAlignment$ Center=�6O   ��6<   �� *A+6O   5x   /    5z   *   �� 5{   .   .R�5|   2   �� #��[ 5}   &   $��Center=�5~   &   $
 True��5   &   ��5|   :   �� *A+$ TwoWay=�$ Explicit=�5   &   5�   /   5�   '     5�   &   $ B(mm)��6=   $ 2* 6L    5�   *   �� 5�   .   .R�5�   2   �� $ Center=�6O   ��6<   �� *B+6O   5�   /    5�   *   �� 5�   .   .R�5�   2   �� #��[ 5�   &   $��Center=�5�   &   $
 True��5�   &   ��5�   :   �� *B+$ TwoWay=�$ Explicit=�{�5�   &   �� *Cells[1].IsEnabled+5�   &   5�   /   5�   '   5�      5�      5�      �� 5�      $��Tọa độ cáp��6   $��75��62   #��[ 5�      .��5�        $��4��6_   $��0,0,5,5q�6l   $ False��6}   $ False��5�      $ False��5�      $	 All=�5�      ���	#FFB1B1B1
�������
���   �$��Stretch=�5�      $��Stretch=�5�      $ False��5�      ��5�   #   �� *%#SelectedTendonProfile.WrappedPoints+ 5�      0  *System.Windows.Controls.DataGridTextColumn  5�   "   $ Global X(mm)��65   $
 auto 6b      
IsReadOnly$
 True��6o   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6K   �� *X+6o     5�   "   $ Global Y(mm)��65   $
 Auto 6a    6K   �� *Y+6p     5�   "   $ Global Z(mm)��65   $
 Auto 6a    6K   �� *Z+6p   5�      5�      �� 5�      $��Điều chỉnh��6   $��75��62   #��[ 5�      .��5�      �  5�      �� 5�   "   $
��auto �63   6B   �� 5�   "   $
��auto �63   6B   �� 5�   "   $
��auto �63   6B   �� 5�   "   $
��auto �63   6B   �� 5�   "   $
��auto �63   6B    5�      �� 5�   "   $
��auto �60   6@   �� 5�   "   $
��auto �60   6@   �� 5�   "   62   .+�5�      �� $��	10,10,0,0q�6)   $��Offset dầm ��6<   #��[ 6O   $��20��6{   6�   �� 5�      $��1��6&   #��[ 66   $��75��6_   $��	10,10,0,0q�6j   6   �� 5�      $��2��6(   $��	10,10,0,0q�69   $��Offset sàn ��6L   #��[ 6_   $��20��6�   6�   �� 5�      $��3��6&   #��[ 66   $��75��6_   $��	10,10,0,0q�6j   6   �� 5�      $��4��6%   $��
10,10,10,0q�65   $��80��6I   #��[  6T   ��6|   �� .��6�   Cập nhập6�   5�      5�      5�      5�      5�      5�      5�      