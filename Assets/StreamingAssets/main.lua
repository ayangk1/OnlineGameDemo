print('this is main')
--define
GameObject = CS.UnityEngine.GameObject
PrimitiveType = CS.UnityEngine.PrimitiveType
Debug = CS.UnityEngine.Debug
Vector3 = CS.UnityEngine.Vector3
Vector2 = CS.UnityEngine.Vector2
--prefab
player = GameObject.CreatePrimitive(PrimitiveType.Cube)
--AddComponent
player:AddComponent(typeof(CS.UnityEngine.Rigidbody))
player:AddComponent(typeof(CS.Character))
--attribute
local playerInput = CS.PlayerInput()
--GetComponent("Character") 无约束 有参数
character = player:GetComponent(typeof(CS.Character))
MeshRenderer = player:GetComponent(typeof(CS.UnityEngine.MeshRenderer))
--泛型需要转为通用方法
moveInput = CS.UnityEngine.InputSystem.InputAction()
--ReadValue<Vector2>() 有约束 无参数
moveInputgeneric = xlua.get_generic_method(CS.UnityEngine.InputSystem.InputAction,"ReadValue")
moveInputReadValue = moveInputgeneric(Vector2)

asset = CS.UnityEngine.AssetBundle.LoadFromFile(CS.System.IO.Path.Combine(CS.UnityEngine.Application.streamingAssetsPath,"mat"));
--LoadAsset<Material>("LuaMat");有约束 有参数的
mat = asset:LoadAsset("LuaMat",typeof(CS.UnityEngine.Material))
-- MeshRenderer.materials[0].color = CS.UnityEngine.Color.blue
MeshRenderer.material = mat
print(mat.name)

player.transform.position = CS.UnityEngine.Vector3.up
moveInput = playerInput.Keyboard.Move

--生命周期函数
LifeCycleTable = {}

function LifeCycleTable:Start()
    character.moveSpeed = 3
    playerInput.Keyboard:Enable()
end

function LifeCycleTable:Update()
    local movedir = moveInputReadValue(moveInput,typeof(Vector2))
    if movedir ~= Vector2.zero then
        local ve = Vector3(movedir.x,0,movedir.y);
        player.transform:Translate(ve * CS.UnityEngine.Time.deltaTime * character.moveSpeed)
    end
end

function LifeCycleTable:OnDestroy()
    print("lua OnDestroy")
end
--生命周期函数
-- local util = require 'xlua.util'
-- util.print_func_ref_by_csharp()


