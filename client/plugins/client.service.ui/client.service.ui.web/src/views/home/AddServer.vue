<template>
    <el-dialog title="添加服务器节点" top="1vh" destroy-on-close v-model="state.show" center :close-on-click-modal="false" width="300px" @click.stop>
        <el-form ref="formDom" :model="state.form" :rules="state.rules" label-width="75px" @click.stop>
            <el-form-item label="域名/ip" prop="Ip">
                <el-input v-model="state.form.Ip"></el-input>
            </el-form-item>
            <el-form-item label="tcp端口" prop="TcpPort">
                <el-input v-model="state.form.TcpPort"></el-input>
            </el-form-item>
            <el-form-item label="udp端口" prop="UdpPort">
                <el-input v-model="state.form.UdpPort"></el-input>
            </el-form-item>

            <el-form-item label="地区标识" prop="Img">
                <el-select v-model="state.form.Img" placeholder="Select">
                    <template #prefix>
                        <img v-if="state.form.Img" :src="shareData.serverImgs[state.form.Img].img" alt="" height="20">
                    </template>
                    <el-option v-for="(item,key) in shareData.serverImgs" :key="key" :label="key" :value="key">
                        <img :src="item.img" alt="" height="20" style="vertical-align: middle;">
                        <span>{{item.name}}</span>
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="备注名称" prop="Name">
                <el-input v-model="state.form.Name"></el-input>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="handleCancel">取 消</el-button>
            <el-button type="primary" :loading="state.loading" @click="handleSubmit">确 定</el-button>
        </template>
    </el-dialog>
</template>

<script>
import { reactive, ref } from '@vue/reactivity';
import { inject, watch } from '@vue/runtime-core';
import { getSignInInfo, updateConfig } from '../../apis/signin'
import { injectShareData } from '../../states/shareData'
import { ElMessage } from 'element-plus/lib/components';
export default {
    props: ['modelValue'],
    emits: ['update:modelValue', 'success'],
    setup(props, { emit }) {

        const shareData = injectShareData();
        const stateEdit = inject('add-edit-data');
        const state = reactive({
            show: props.modelValue,
            loading: false,
            form: {
                TcpPort: stateEdit.value.data.TcpPort || 5410,
                UdpPort: stateEdit.value.data.UdpPort || 5410,
                Ip: stateEdit.value.data.Ip || '',
                Name: stateEdit.value.data.Name || '',
                Img: stateEdit.value.data.Img || Object.keys(shareData.serverImgs)[0],
            },
            rules: {
                Ip: [{ required: true, message: '必填', trigger: 'blur' }],
                TcpPort: [
                    { required: true, message: '必填', trigger: 'blur' },
                    {
                        type: 'number', min: 1, max: 65535, message: '数字 1-65535', trigger: 'blur', transform(value) {
                            return Number(value)
                        }
                    }
                ],
                UdpPort: [
                    { required: true, message: '必填', trigger: 'blur' },
                    {
                        type: 'number', min: 1, max: 65535, message: '数字 1-65535', trigger: 'blur', transform(value) {
                            return Number(value)
                        }
                    }
                ],
            }
        });
        watch(() => state.show, (val) => {
            if (val == false) {
                setTimeout(() => {
                    emit('update:modelValue', val);
                }, 300);
            }
        });

        const formDom = ref(null);
        const handleSubmit = () => {
            formDom.value.validate((valid) => {
                if (valid == false) {
                    return false;
                }
                state.loading = true;

                getSignInInfo().then((json) => {
                    if (json.ServerConfig.Items.filter((c, index) => c.Ip == state.form.Ip && index != stateEdit.value.index).length > 0) {
                        state.loading = false;
                        ElMessage.error('已存在相同地址');
                        return;
                    }
                    if (stateEdit.value.index == -1) {
                        json.ServerConfig.Items.push({
                            Img: state.form.Img,
                            Name: state.form.Name,
                            Ip: state.form.Ip,
                            TcpPort: Number(state.form.TcpPort),
                            UdpPort: Number(state.form.UdpPort),
                        });
                    } else {
                        let item = json.ServerConfig.Items[stateEdit.value.index];
                        item.Img = state.form.Img;
                        item.Name = state.form.Name;
                        item.Ip = state.form.Ip;
                        item.TcpPort = Number(state.form.TcpPort);
                        item.UdpPort = Number(state.form.UdpPort);
                    }

                    updateConfig(json).then(() => {
                        state.show = false;
                        emit('success');
                    }).catch(() => {
                        state.show = false;
                        ElMessage.error('选择失败');
                    });
                }).catch(() => {
                    state.show = false;
                });
            })
        }
        const handleCancel = () => {
            state.show = false;
        }

        return {
            shareData, state, formDom, handleSubmit, handleCancel
        }
    }
}
</script>
<style lang="stylus" scoped>
.remark {
    margin-top: 1rem;
}
</style>