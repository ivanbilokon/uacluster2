﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.IO;
using VMClusterManager.Controls.Dialogs;
using System.Windows.Threading;
using System.Management;

namespace VMClusterManager
{
    public class VMModel
    {


        public Dispatcher UIDispatcher;
        #region IVMModel Members
        private VM activeVM;

        public VM ActiveVM
        {
            get
            {
                return activeVM;
            }
            set
            {
                activeVM = value;
            }
        }

        public VMGroup RootVMGroup
        {
            get { return rootVMGroup; }
        }

        public VMGroup ActiveVMGroup
        {
            get { return activeVMGroup; }
            set
            {
                activeVMGroup = value;
                OnActiveVMGroupChanged();
            }
        }

        #endregion

        #region VMs
        private ObservableCollection<VM> activeVMList;

        private object selectedTreeItem;

        public object SelectedTreeItem
        {
            get { return selectedTreeItem; }
            set 
            { 
                selectedTreeItem = value;
                OnSelectedTreeItemChanged(selectedTreeItem);
            }
        }

        private VMGroup rootVMGroup;
        private VMGroup activeVMGroup;
        public ObservableCollection<VM> ActiveVMList
        {
            get { return activeVMList; }
            set
            {
                activeVMList = value;
            }
        }

        private object selectedVMItem;

        public object SelectedVMItem
        {
            get { return selectedVMItem; }
            set 
            { 
                selectedVMItem = value;
                OnSelectedVMListItemChanged(selectedVMItem);
            }
        }

        private object selectedSnapshotItem;

        public object SelectedSnapshotItem
        {
            get { return selectedSnapshotItem; }
            set { selectedSnapshotItem = value; OnSelectedSnapshotItemChanged(selectedSnapshotItem); }
        }


    
        #endregion

        # region HOSTS
        private VMHost activeVMHost;

        public VMHost ActiveVMHost
        {
            get
            {
                return activeVMHost;
            }
            set
            {
                activeVMHost = value;
            }
        }
        public List<VMHost> VMHostList
        {
            get { throw new NotImplementedException(); }
        }


        private ObservableCollection<VMHost> activeVMHostList;
        public ObservableCollection<VMHost> ActiveVMHostList
        {
            get
            {
                return activeVMHostList;
            }
            set
            {
                activeVMHostList = value;
                OnActiveVMHostListChanged();
            }
        }



        private VMHostGroup rootVMHostGroup;
        public VMHostGroup RootVMHostGroup
        {
            get { return rootVMHostGroup; }
        }

        private VMHostGroup activeVMHostGroup;
        public VMHostGroup ActiveVMHostGroup
        {
            get
            {
                return activeVMHostGroup;
            }
            set
            {
                activeVMHostGroup = value;
                OnActiveVMHostGroupChanged();
            }
        }

        public event EventHandler<ActiveVMHostGroupChangedEventArgs> ActiveVMHostGroupChanged;
        protected void OnActiveVMHostGroupChanged()
        {
            if (ActiveVMHostGroupChanged != null)
                ActiveVMHostGroupChanged(this, new ActiveVMHostGroupChangedEventArgs());
        }

        public event EventHandler<ActiveVMHostListChangedEventArgs> ActiveVMHostListChanged;
        protected void OnActiveVMHostListChanged()
        {
            if (ActiveVMHostListChanged != null)
            {
                ActiveVMHostListChanged(this, new ActiveVMHostListChangedEventArgs());
            }
        }

        public event EventHandler<VMHostTreeChangedEventArgs> VMHostTreeChanged;
        protected void OnVMHostTreeChanged()
        {
            if (VMHostTreeChanged != null)
                VMHostTreeChanged(rootVMHostGroup, new VMHostTreeChangedEventArgs());
        }

        #endregion

        private static VMModel inst;

        public static VMModel GetInstance()
        {
            if (inst == null)
                inst = new VMModel();
            return inst;
        }

        //XElement VMTreeStructure = null;

        private Settings settings;

        public Settings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        private static string SettingsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + 
            System.Windows.Forms.Application.ProductName;

        private static string VMTreeFileName = SettingsFolderPath  + "\\" + "VMTree.xml";
        private static string VMHostTreeFileName = SettingsFolderPath + "\\" + "HostTree.xml";
        private static string SettingsFileName = SettingsFolderPath + "\\" + "Settings.xml";

        private VMModel()
        {
            try
            {
                UIDispatcher = Dispatcher.CurrentDispatcher;
                if (!Directory.Exists(SettingsFolderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(SettingsFolderPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, SettingsFolderPath, MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                Settings.SettingsFileName = SettingsFileName;
                try
                {
                    Settings = Settings.LoadFromFile();
                }
                catch (Exception ex)
                {
                    Settings = new Settings();
                    try
                    {
                        Settings.SaveToFile();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Error creating settings file", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                DataReceiver DR = new DataReceiver();
                //rootVMHostGroup = new VMHostGroup("All hosts");
                VMHostGroup.Storage = VMHostTreeFileName;
                if (!File.Exists(VMHostTreeFileName))
                {
                    VMHostGroup temp = new VMHostGroup("All hosts");
                    temp.SaveToXML(VMHostGroup.Storage.ToString());

                }
                try
                {

                    rootVMHostGroup = new VMHostGroup(XElement.Load(VMHostGroup.Storage.ToString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error loading VMHost group structure", MessageBoxButton.OK, MessageBoxImage.Error);
                    rootVMHostGroup = new VMHostGroup("All hosts");
                }
                VMGroup.Storage = VMTreeFileName;
                //retrieve VM Tree structure
                if (!File.Exists(VMTreeFileName))
                {
                    VMGroup temp = new VMGroup("All VM");
                    temp.SaveToXML(VMGroup.Storage.ToString());

                }
                try
                    {

                        rootVMGroup = new VMGroup(XElement.Load(VMGroup.Storage.ToString()));
                    }
                catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error loading VM group structure", MessageBoxButton.OK, MessageBoxImage.Error);
                        rootVMGroup = new VMGroup("All VM");
                    }
                    
               
                ////---------------------------
                VMHostGroupEventSubscriber(rootVMHostGroup);
                //MessageBox.Show(, MessageBoxButton.OK);
                activeVMList = new ObservableCollection<VM>();
                ActiveVMList.CollectionChanged +=
                                       (o, e) =>
                                       {
                                           OnActiveVMListChanged(e);
                                       };
                ActiveVMHostList = new ObservableCollection<VMHost>();
                ActiveVMHostList.CollectionChanged +=
                    (o, e) =>
                    {
                        OnActiveVMHostListChanged();
                    };
                activeVM = null;
                activeVMGroup = null;
                DR.FillVMHostTree(RootVMHostGroup.DataObject, RootVMHostGroup);
                //List<VMHost> hostList = DR.GetHostListFromFile("hostlist.txt");
                //foreach (VMHost host in hostList)
                //{
                //    rootVMHostGroup.AddHost(host);
                //}
                
                
                //dlg.Close();
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "VMModel()", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Recursively walks around tree of host groups and subscribes for event
        /// </summary>
        /// <param name="group"></param>
        private void VMHostGroupEventSubscriber(VMHostGroup group)
        {
            if (group != null)
            {
                group.HostListChanged +=
                    (o, e) =>
                    {
                        if (e.IsHostAdded)
                        {
                            if (e.InvolvedHost != null)
                            {
                                Thread vmListReceiver = new Thread(new ParameterizedThreadStart(GetVMList));
                                vmListReceiver.IsBackground = true;
                                vmListReceiver.Start(e.InvolvedHost);
                            }
                        }
                        OnVMHostTreeChanged();

                    };
                group.ChildGroupsChanged +=
                    (o, e) =>
                    {
                        VMHostGroupEventSubscriber(e.InvolvedGroup);
                        OnVMHostTreeChanged();
                    };
                if (group.ChildGroups != null)
                {
                    foreach (VMHostGroup grp in group.ChildGroups)
                    {
                        VMHostGroupEventSubscriber(grp);
                    }
                }
            }
        }

        ConnectionOptions NewAuth(VMHost host)
        {
            AuthenticationDialog dlg = new AuthenticationDialog("Login to " + host.Name);
            dlg.ShowDialog();
            ConnectionOptions connOpts = null;
            if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
            {
                connOpts = new ConnectionOptions();
                connOpts.Authentication = AuthenticationLevel.Call;
                connOpts.SecurePassword = dlg.Password;
                connOpts.Username = dlg.UserName;
            }
            return connOpts;
        }

        private int GetVMListCounter = 0;

        private void GetVMList(object param)
        {
            Interlocked.Increment(ref GetVMListCounter);
            try
            {

                if (param is VMHost)
                {
                    VMHost host = param as VMHost;
                    while (true)
                    {
                        try
                        {
                            host.Connect();
                            break;
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            ConnectionOptions opts = (ConnectionOptions)UIDispatcher.Invoke(new Func<VMHost, ConnectionOptions>(NewAuth), host);
                            if (opts != null)
                            {
                                host.HostConnectionOptions = opts;
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (RPCCallException ex)
                        {
                            MessageBox.Show(ex.Message, host.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        }
                        catch (ManagementException ex)
                        {
                            MessageBox.Show(ex.Message, host.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        }

                    }

                    ObservableCollection<VM> vmColl = host.VMCollection;
                    if (vmColl != null)
                    {
                        foreach (VM vm in vmColl)
                        {
                            VMGroup parentForVM = DataReceiver.FindParentForVM(vm, RootVMGroup.DataObject, RootVMGroup);
                            if (parentForVM != null)
                            {
                                parentForVM.AddVM(vm);
                            }
                            else
                            {
                                rootVMGroup.AddVM(vm);
                            }
                            OnVMHostTreeChanged();

                        }
                    }

                }
                //lock (VMGroup.Storage)
                //{
                //    RootVMGroup.Save();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, (param as VMHost).Name);
            }
            finally
            {
                Interlocked.Decrement(ref GetVMListCounter);
            }
            lock ((object)GetVMListCounter)
            {
                if (GetVMListCounter == 0)
                {
                    try
                    {

                        RootVMGroup.Save();
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message, "Saving VM group structure...", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            
        }

        private void VMHostGroupEventSubscriber(ChildHostGroupsChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #region IVMModel Members


        public event EventHandler<VMStateChangedEventArgs> VMStateChanged;

        protected void OnVMStateChanged()
        {
            if (VMStateChanged != null)
                VMStateChanged(this, new VMStateChangedEventArgs());
        }

        /// <summary>
        /// Occurs when Active element of Tree View changes
        /// </summary>
        public event EventHandler<SelectedItemChangedEventArgs> SelectedTreeItemChanged;

        protected void OnSelectedTreeItemChanged(object item)
        {
            if (SelectedTreeItemChanged != null)
                SelectedTreeItemChanged(this, new SelectedItemChangedEventArgs(item));
        }


        public event EventHandler<NotifyCollectionChangedEventArgs> ActiveVMListChanged;

        protected void OnActiveVMListChanged(NotifyCollectionChangedEventArgs e)
        {
            if (ActiveVMListChanged != null)
                ActiveVMListChanged(this,e);
        }

    

        public event EventHandler<ActiveVMGroupChangedEventArgs> ActiveVMGroupChanged;

        protected void OnActiveVMGroupChanged()
        {
            if (ActiveVMGroupChanged != null)
                ActiveVMGroupChanged(this, new ActiveVMGroupChangedEventArgs());
        }



        public event EventHandler<SelectedItemChangedEventArgs> SelectedVMListItemChanged;

        protected void OnSelectedVMListItemChanged(object item)
        {
            if (SelectedVMListItemChanged != null)
            {
                SelectedVMListItemChanged(this, new SelectedItemChangedEventArgs(item));
            }
        }

        #endregion
        public event EventHandler<EventArgs> SelectedSnapshotItemChanged;
        private void OnSelectedSnapshotItemChanged(object selectedSnapshotItem)
        {
            if (SelectedSnapshotItemChanged != null)
                SelectedSnapshotItemChanged(this, new EventArgs());
        }

        //public event EventHandler<EventArgs> HostListInitialized;
        //private void OnHostListInitialized(IEnumerable<VMHost> hostlist)
        //{
           

        //    if (HostListInitialized!= null)
        //    {
        //        HostListInitialized(this, new EventArgs());
        //    }
        //}

        #region API functions

        public Group CreateGroup(Group parent)
        {
            const string DefaultGroupName = "New Group";
            string newGroupName = DefaultGroupName;
            //if parent group already contains group named as DefaultGroupName then add to DefaultGroupName 1, 2,... 
            for (int i = 1; (from g in parent.ChildGroups where g.Name == newGroupName select g).Count() > 0; i++)
            {
                newGroupName = DefaultGroupName + " " + i.ToString();
            }
            return parent.CreateGroup(newGroupName);
        }

        public void RemoveGroup(VMGroup group)
        {
            if (group != null)
            {
                if (group.Children.Count == 0)
                {
                    if (MessageBox.Show("Are you shure you want to remove this group?", group.Name,
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                    {
                        VMGroup parentGroup = group.ParentGroup as VMGroup;
                        group.Remove();
                        parentGroup.Save();
                    }
                }
                else
                {
                    MessageBox.Show("Group is not empty! You can delete only empty groups.", group.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void RemoveGroup(VMHostGroup group)
        {
            if (group != null)
            {
                if (group.Children.Count == 0)
                {
                    if (MessageBox.Show("Are you shure you want to remove this group?", group.Name,
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                    {
                        VMHostGroup parentGroup = group.ParentGroup as VMHostGroup;
                        group.Remove();
                        parentGroup.Save();
                    }
                }
                else
                {
                    MessageBox.Show("Group is not empty! You can delete only empty groups.", group.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private const int JobWaitTime = 100;

        delegate void AddLogMessageDelegate(LogMessage message);

        /// <summary>
        /// Provides an action on specific collection of VM
        /// </summary>
        /// <param name="vmList"></param>
        /// <param name="action"></param>
        private void VMListAction(ICollection<VM> vmList, Func<VM, VMJob> action)
        {
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    Dispatcher UIDispatcher = Dispatcher.CurrentDispatcher;
                    Thread vmThread = new Thread(new ParameterizedThreadStart(delegate(object param)
                    {
                        
                        try
                        {

                            VMJob ThisJob = action(param as VM);
                            if (ThisJob != null)
                            {
                                while ((ThisJob.JobState == JobState.Starting) || (ThisJob.JobState == JobState.Running))
                                {
                                    Thread.Sleep(JobWaitTime);
                                }

                                if (ThisJob.JobState != JobState.Completed)
                                {
                                    if (vmList.Count == 1)
                                    {
                                        MessageBox.Show(ThisJob.GetError(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        //this is done to allow this thread to change collection which is monitored by user interface main thread
                                        //otherwise an error occures
                                        UIDispatcher.Invoke(new AddLogMessageDelegate(
                                            delegate(LogMessage message)
                                            {
                                                VMLog.GetInstance().Add(message);
                                            }
                                            ), new LogMessage(LogMessageTypes.Error, ThisJob.GetError(), (param as VM).Name, ThisJob.Caption));
                                    }
                                }
                                
                            }
                        }
                        catch (VMOperationException ex)
                        {
                           
                            UIDispatcher.Invoke(new AddLogMessageDelegate(
                                        delegate(LogMessage message)
                                        {
                                            VMLog.GetInstance().Add(message);
                                        }
                                        ), new LogMessage(LogMessageTypes.Error, ex.Message, (param as VM).Name, action.Method.Name));
                        }
                    }));
                    vmThread.IsBackground = true;
                    vmThread.Start(vm);
                }
            }
        }

        public void StartVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Start();
            }
            );

        }

        public bool CanStartVMList(ObservableCollection<VM> vmList)
        {
            bool canStart = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStart = (vm.Status == VMState.Disabled || vm.Status == VMState.Paused ||
                        vm.Status == VMState.Suspended);
                    if (canStart) return canStart;
                }
            }
            return canStart;
        }

        public void StopVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Stop();
            }
            );

        }

        public void ShutdownVMList(ObservableCollection<VM> vmList)
        {
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    Dispatcher UIDispatcher = Dispatcher.CurrentDispatcher;
                    Thread vmThread = new Thread(new ParameterizedThreadStart(delegate(object param)
                    {
                        try
                        {

                            uint returnValue = vm.Shutdown();
                            if (returnValue != 0)
                            {
                                if (vmList.Count == 1)
                                {
                                    MessageBox.Show("Failed to shutdown selected VM" + Environment.NewLine + "Error code: " + returnValue.ToString()
                                        , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                else
                                {
                                    //this is done to allow this thread to change collection which is monitored by user interface main thread
                                    //otherwise an error occures
                                    UIDispatcher.Invoke(new AddLogMessageDelegate(
                                        delegate(LogMessage message)
                                        {
                                            VMLog.GetInstance().Add(message);
                                        }
                                        ), new LogMessage(LogMessageTypes.Error, "Failed to shutdown selected VM. Error code: " + returnValue.ToString(), (param as VM).Name, "Shutdown"));
                                }
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            UIDispatcher.Invoke(new AddLogMessageDelegate(
                                        delegate(LogMessage message)
                                        {
                                            VMLog.GetInstance().Add(message);
                                        }
                                        ), new LogMessage(LogMessageTypes.Warning, ex.Message, (param as VM).Name, "Shutdown"));
                        }
                    }));
                    vmThread.IsBackground = true;
                    vmThread.Start(vm);
                }
            }
        }

        public void SetMemoryVMList(ICollection<VM> vmList, UInt64 quantity)
        {
            VMListAction(vmList,
                delegate(VM vm)
                {
                    return Utility.SetMemory(vm, quantity);
                });
        }

        public void SetProcessorVMList(ICollection<VM> vmList, UInt64 quantity)
        {
            VMListAction(vmList,
               delegate(VM vm)
               {
                   return Utility.SetProcessor(vm, quantity);
               });
        }

        public bool CanShutdownVMList(ObservableCollection<VM> vmList)
        {
            return CanStopVMList(vmList);
        }

        public bool CanStopVMList(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = !(vm.Status == VMState.Disabled || vm.Status == VMState.Stopping || vm.Status == VMState.Suspended);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }

        public void SuspendVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Suspend();
            }
            );

        }

        public bool CanSuspendVMList(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = (vm.Status == VMState.Enabled || vm.Status == VMState.Paused);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }

        public void PauseVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Pause();
            }
            );

        }

        public bool CanPauseVMList(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = (vm.Status == VMState.Enabled || vm.Status == VMState.Resuming);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }

        public void RebootVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Reboot();
            }
            );

        }

        public bool CanRebootVMList(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = !(vm.Status == VMState.Disabled || vm.Status == VMState.Suspended);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }


        public void CreateSnapshot(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.CreateSnapshot();
            }
            );

        }

        public bool CanCreateSnapshot(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = (vm.Status == VMState.Disabled || vm.Status == VMState.Suspended || vm.Status == VMState.Enabled);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }

        public void ApplySnapshot(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, (vm) =>
            {
                return vm.ApplySnapshot(snapshot);
            });
        }

        public bool CanApplySnapshot(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            bool canApply = false;
            if (vmList != null)
            {
                if ((vmList.Count == 1) && snapshot != null)
                {
                    if (vmList[0].Status == VMState.Disabled || vmList[0].Status == VMState.Suspended)
                    {
                        canApply = true;
                    }
                }
            }
            return canApply;
        }

        public void RemoveSnapshot(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, (vm) =>
            {
                return vm.RemoveSnapshot(snapshot);
            });
            
        }

        public bool CanRemoveSnapshot(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            bool canRemove = false;
            if (vmList != null)
            {
                if ((vmList.Count == 1) && snapshot != null)
                {
                    canRemove = true;
                }
            }
            return canRemove;
        }

        public void RemoveSnapshotTree(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            
                VMListAction(vmList, (vm) =>
                {
                    return vm.RemoveSnapshotTree(snapshot);
                });
        
        }

        public bool CanRemoveSnapshotTree(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            bool canRemove = false;
            if (vmList != null)
            {
                if ((vmList.Count == 1) && snapshot != null)
                {
                    canRemove = true;
                }
            }
            return canRemove;
        }



        public void MoveToGroup(VM item, VMGroup newParent)
        {
            lock (item)
            {
                VMGroup parentGroup = VMGroup.FindParentFor(item, this.RootVMGroup);
                if (parentGroup != newParent)
                {
                    lock (newParent)
                    {
                        newParent.AddVM(item);
                    }
                    lock (parentGroup)
                    {
                        parentGroup.RemoveVM(item);
                    }
                }
            }
        }

        public void MoveToGroup(VMHost item, VMHostGroup newParent)
        {
            lock (item)
            {
                VMHostGroup parentGroup = VMHostGroup.FindParentFor(item, this.RootVMHostGroup);
                if (parentGroup != newParent)
                {
                    lock (newParent)
                    {
                        newParent.AddHost(item);
                    }
                    lock (parentGroup)
                    {
                        parentGroup.RemoveHost(item);
                    }
                }
            }
        }

        public void MoveToGroup(Group item, Group newParent)
        {
            lock (item)
            {
                item.MoveTo(newParent);
            }
        }


        public void MoveToGroup(ObservableCollection<VM> vmList, VMGroup group)
        {
            
                ObservableCollection<VM> VMToMove = new ObservableCollection<VM>(vmList);
                foreach (VM vm in VMToMove)
                {
                    MoveToGroup(vm, group);
                }
            
        }

        public void MoveToGroup(ObservableCollection<VMHost> vmHostList, VMHostGroup group)
        {

            ObservableCollection<VMHost> VMHostToMove = new ObservableCollection<VMHost>(vmHostList);
            foreach (VMHost vmHost in VMHostToMove)
            {
                MoveToGroup(vmHost, group);
            }

        }

        public bool CanMoveToGroup(object item)
        {
            bool canMove = false;
            if (item != null)
            {
                if (item is VMGroup)
                {
                    if ((item as VMGroup).ParentGroup != null)
                    {
                        canMove = true;
                    }
                                    }
                //if (item is VM)
                //{
                //    canMove = true;
                //}
                if (item is VMHostGroup)
                {
                    canMove = true;
                }
            }
            return canMove;
        }


        public bool CanMoveToGroup(ObservableCollection<VM> vmList)
        {
            bool canMove = false;
            if (vmList != null)
            {
                if (vmList.Count > 0)
                {
                    canMove = true;
                }
            }
            return canMove;
        }

        public bool CanMoveToGroup(ObservableCollection<VMHost> vmHostList)
        {
            bool canMove = false;
            if (vmHostList != null)
            {
                if (vmHostList.Count > 0)
                {
                    canMove = true;
                }
            }
            return canMove;
        }

        public void RemoveHost(VMHost host)
        {
            //Remove host's VM from its parent groups
            if (MessageBox.Show("Warning! If you remove this host you won't be able to control Virtual Machines hosted on it." + Environment.NewLine + "Are you shure you want to remove this Host?", host.Name,
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                foreach (VM vm in host.VMCollection)
                {
                    VMGroup vmParent = VMGroup.FindParentFor(vm, RootVMGroup);
                    vmParent.RemoveVM(vm);
                    vmParent.Save();
                }

                VMHostGroup hostParent = VMHostGroup.FindParentFor(host, RootVMHostGroup);
                hostParent.RemoveHost(host);
                hostParent.Save();
            }
        }

        #endregion //API functions

        public static string GetItemName<T>(T item) where T : class
        {
            var properties = typeof(T).GetProperties();
            //Enforce.That(properties.Length == 1);
            return properties[1].Name;
        }

    }

    

}
