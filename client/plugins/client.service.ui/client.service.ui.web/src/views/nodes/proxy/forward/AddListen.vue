<template>
    <el-dialog :title="state.form.ID > 0?'编辑监听':'新增监听'" top="1vh" destroy-on-close v-model="state.show" center :close-on-click-modal="false" width="260px">
        <el-form ref="formDom" :model="state.form" :rules="state.rules" label-width="80px">
            <el-form-item label="监听端口" prop="Port">
                <el-input v-model="state.form.Port" :readonly="state.form.ID > 0"></el-input>
            </el-form-item>
            <el-form-item label="连接类型" prop="AliveType">
                <el-select v-model="state.form.AliveType" placeholder="选择类型">
                    <el-option v-for="(item,index) in shareData.aliveTypes" :key="index" :label="item" :value="index">
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="" prop="">
                http的选啥类型都可以，其它的选长连接
            </el-form-item>
            <el-form-item label="简单说明" prop="Desc">
                <el-input v-model="state.form.Desc"></el-input>
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="handleCancel">取 消</el-button>
            <el-button type="primary" :loading="state.loading" @click="handleSubmit">确 定</el-button>
        </template>
    </el-dialog>
</template>

<script>
import { reactive, ref, } from '@vue/reactivity';
import { addListen } from '../../../../apis/forward'
import { injectShareData } from '../../../../states/shareData'
import { inject, watch } from '@vue/runtime-core';
export default {
    props: ['modelValue'],
    emits: ['update:modelValue', 'success'],
    setup(props, { emit }) {
        const addListenData = inject('add-listen-data');
        const shareData = injectShareData();
        const state = reactive({
            show: props.modelValue,
            loading: false,
            form: {
                ID: addListenData.value.ID || 0,
                Port: addListenData.value.Port || 0,
                AliveType: (addListenData.value.AliveType || shareData.aliveTypesName.tunnel) + '',
                Desc: addListenData.value.Desc || ''
            },
            rules: {
                Port: [
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
            if (!val) {
                setTimeout(() => {
                    emit('update:modelValue', val);
                }, 300);
            }
        });

        const formDom = ref(null);
        const handleSubmit = () => {
            formDom.value.validate((valid) => {
                if (!valid) {
                    return false;
                }
                state.loading = true;

                const json = JSON.parse(JSON.stringify(state.form));
                json.Port = Number(json.Port);
                json.AliveType = Number(json.AliveType);
                addListen(json).then(() => {
                    state.loading = false;
                    state.show = false;
                    emit('success');
                }).catch((e) => {
                    state.loading = false;
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
<style lang="stylus" scoped></style>