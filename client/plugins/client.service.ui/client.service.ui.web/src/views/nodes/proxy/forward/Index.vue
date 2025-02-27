<template>
    <div class="forward-wrap">
        <div class="inner">
            <div class="head flex">
                <el-button type="primary" size="small" @click="handleAddListen">监听</el-button>
                <el-button size="small" @click="getData">刷新</el-button>
            </div>
            <div class="content">
                <el-table :data="state.list" size="small" border>
                    <el-table-column type="expand">
                        <template #default="props">
                            <el-table size="small" :data="props.row.Forwards" border>
                                <el-table-column label="访问" prop="SourceIp">
                                    <template #default="props1">
                                        <span>{{props1.row.SourceIp}}:{{props.row.Port}}</span>
                                    </template>
                                </el-table-column>
                                <el-table-column label="目标节点" prop="TargetIp">
                                    <template #default="props">
                                        <span>{{ targetJson[props.row.ConnectionId]}}</span>
                                    </template>
                                </el-table-column>
                                <el-table-column label="目标服务" prop="TargetIp">
                                    <template #default="props">
                                        <span>{{props.row.TargetIp}}:{{props.row.TargetPort}}</span>
                                    </template>
                                </el-table-column>
                                <el-table-column align="right" width="140">
                                    <template #default="props1">
                                        <el-popconfirm title="删除不可逆，是否确认" @confirm="handleRemoveForward(props.row,props1.row)">
                                            <template #reference>
                                                <el-button link plain type="danger" size="small">删除</el-button>
                                            </template>
                                        </el-popconfirm>
                                        <el-button link plain size="small" @click="handleEditForward(props.row,props1.row)">编辑</el-button>
                                        <el-button link plain size="small" @click="handleTestForward(props.row,props1.row)">测试</el-button>
                                    </template>
                                </el-table-column>
                            </el-table>
                        </template>
                    </el-table-column>
                    <el-table-column label="监听类别" prop="AliveType">
                        <template #default="props">
                            <span>{{shareData.aliveTypes[props.row.AliveType]}}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="监听端口" prop="Port">
                        <template #default="props">
                            <div class="flex">
                                <span class="flex-1">{{props.row.Port}}</span>
                                <span>
                                    <el-switch size="small" @click.stop @change="onListeningChange(props.row)" v-model="props.row.Listening"></el-switch>
                                </span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column label="备注" prop="Desc"></el-table-column>
                    <el-table-column align="right" width="140">
                        <template #default="props">
                            <el-popconfirm title="删除不可逆，是否确认" @confirm="handleRemoveListen(props.row)">
                                <template #reference>
                                    <el-button plain link type="danger" size="small">删除</el-button>
                                </template>
                            </el-popconfirm>
                            <el-button plain type="info" link size="small" @click="handleEditListen(props.row)">编辑</el-button>
                            <el-button plain type="info" link v-if="props.row.AliveType == shareData.aliveTypesName.web || props.row.Forwards.length < 1" size="small" @click="handleAddForward(props.row)">转发</el-button>
                        </template>
                    </el-table-column>
                </el-table>
            </div>
            <AddForward v-if="state.showAddForward" v-model="state.showAddForward" @success="getData"></AddForward>
            <AddListen v-if="state.showAddListen" v-model="state.showAddListen" @success="getData"></AddListen>
            <StatusMsg v-if="state.showStatusMsg" v-model="state.showStatusMsg" :msgCallback="state.statusMsgCallback"></StatusMsg>
        </div>
    </div>
</template>
<script>
import { reactive, ref } from '@vue/reactivity'
import { getList, removeListen, startListen, stopListen, removeForward, testForward } from '../../../../apis/forward'
import { computed, onMounted, provide } from '@vue/runtime-core'
import AddForward from './AddForward.vue'
import AddListen from './AddListen.vue'
import { injectShareData } from '../../../../states/shareData'
import { injectClients } from '../../../../states/clients'
import StatusMsg from '../../../../components/StatusMsg.vue'
import plugin from './plugin'
import { ElMessageBox } from 'element-plus'
export default {
    plugin: plugin,
    components: { AddListen, AddForward, StatusMsg },
    setup() {

        const shareData = injectShareData();
        const state = reactive({
            loading: false,
            list: [],
            currentLsiten: { Port: 0 },
            showAddListen: false,
            showAddForward: false,
            showStatusMsg: false,
            statusMsgCallback: () => 0
        });
        const clientsState = injectClients();
        const targets = computed(() => {
            return [{ id: 0, label: '服务器' }].concat(clientsState.clients.map(c => {
                return { id: c.ConnectionId, label: c.Name }
            }));
        });
        const targetJson = computed(() => {
            return targets.value.reduce((value, item) => {
                value[item.id] = item.label;
                return value;
            }, {})
        });

        const expandKeys = ref([]);
        const getData = () => {
            getList().then((res) => {
                state.list = res;
            });
        };
        const onExpand = (a, b) => {
            expandKeys.value = b.map(c => c.ID);
        }

        const addListenData = ref({ ID: 0 });
        provide('add-listen-data', addListenData);
        const handleAddListen = () => {
            addListenData.value = { ID: 0 };
            state.showAddListen = true;
        }
        const handleEditListen = (row) => {
            addListenData.value = row;
            state.showAddListen = true;
        }

        const handleRemoveListen = (row) => {
            removeListen(row.ID).then(() => {
                getData();
            });
        }
        const onListeningChange = (row) => {
            if (row.Listening == false) {
                stopListen(row.ID).then(getData).catch(getData);
            } else {
                startListen(row.ID).then(getData).catch(getData);
            }
        }

        const addForwardData = ref({ forward: { ID: 0 }, ListenID: 0, currentLsiten: { ID: 0, Port: 0 } });
        provide('add-forward-data', addForwardData);

        const handleAddForward = (row) => {
            addForwardData.value.currentLsiten = row;
            addForwardData.value.forward = { ID: 0 };
            state.showAddForward = true;
        }
        const handleEditForward = (listen, forward) => {
            addForwardData.value.currentLsiten = listen;
            addForwardData.value.forward = forward;
            state.showAddForward = true;
        }
        const handleRemoveForward = (listen, forward) => {
            removeForward(listen.ID, forward.ID).then(() => {
                getData();
            });
        }
        const handleTestForward = (listen, forward) => {
            let host = listen.AliveType == shareData.aliveTypesName.tunnel ? '127.0.0.1' : forward.SourceIp;
            let port = listen.Port;
            ElMessageBox.prompt('不带http://，带端口', '测试', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                inputValue: `${host}:${port}`
            }).then(({ value }) => {
                let arr = value.split(':');
                state.statusMsgCallback = testForward(arr[0], +arr[1]);
                state.showStatusMsg = true;
            }).catch(() => {
            });
        }

        onMounted(() => {
            getData();
        });

        return {
            targetJson, state, shareData, getData, expandKeys, onExpand,
            handleRemoveListen, handleAddListen, handleEditListen, onListeningChange,
            handleAddForward, handleEditForward, handleRemoveForward, handleTestForward
        }
    }
}
</script>
<style lang="stylus">
.el-dropdown-menu__item.forward-success {
    color: green;
}

.el-dropdown-menu__item.forward-error {
    color: red;
}
</style>
<style lang="stylus" scoped>
@media screen and (max-width: 500px) {
    .el-col-24 {
        max-width: 100%;
        flex: 0 0 100%;
    }
}

.forward-wrap {
    padding: 2rem;

    .inner {
        background-color: #fff;
        border-radius: 4px;
        border: 1px solid #ddd;
        box-shadow: 0 0 8px 1px rgba(0, 0, 0, 0.05);
    }

    .head {
        padding: 1rem;
        border-bottom: 1px solid #eee;
    }

    .content {
        padding: 1rem;
    }

    .forward-status {
        color: red;
    }

    .forward-status-0 {
        color: green;
    }
}
</style>